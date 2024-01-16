
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Services.TreeServices;
using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.AreaDtos;
using SupervisorMobility.API.Services;
using System.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using SupervisorMobility.API.DataAccess.Entities.Paths;
using FuzzyString;
using SupervisorMobility.API.Models.AssyChart;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
using SupervisorMobility.API.Models.OperationDtos;
using Slugify;
using SupervisorMobility.API.Models.DistributionDtos;

namespace SupervisorMobility.API.Controllers
{
    [Route("api/plants/{plantId}/areas")]
    [ApiController]
    public class AreasController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ISupervisorMobilityRepository _supervisorMobilityRepository;
        private readonly IAssyChartService _assyChartService;
        private readonly IEmailService _email;
        private readonly ITreeService _treeService;
        private readonly CustomHttpClientService customHttp;

        public AreasController(ISupervisorMobilityRepository supervisorMobilityRepository,
            IMapper mapper, IAssyChartService assyChartService, IEmailService email, ITreeService treeService, CustomHttpClientService customHttpClientService)
        {
            _supervisorMobilityRepository = supervisorMobilityRepository ??
                throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _assyChartService = assyChartService ??
                throw new ArgumentNullException(nameof(assyChartService));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
            _email = email ?? throw new ArgumentNullException(nameof(email));
            _treeService = treeService;
            customHttp = customHttpClientService;
        }

        [HttpPost("Testing")]
        public async Task ProcessTreeDataAsync(int plantId, string FileNameForStorage, int UserIdUpload)
        {


            var _bridgeHttpClient = customHttp.GetBridgeHttpClient();

            User userEntity = await _supervisorMobilityRepository.GetUserAsync(UserIdUpload, false);

            IEnumerable<Plant> Plants = await _supervisorMobilityRepository.GetPlantsAsync();
            IEnumerable<Product> Products = await _supervisorMobilityRepository.GetProductsAsync();

            Dictionary<int, Plant> PlantsDictionary = new Dictionary<int, Plant>();
            Dictionary<(int, int), Area> AreasDictionary = new Dictionary<(int, int), Area>();
            Dictionary<(int, int, int), Distribution> DistributionsDictionary = new Dictionary<(int, int, int), Distribution>();
            Dictionary<(int, int, int, int), Operation> OperationsDictionary = new Dictionary<(int, int, int, int), Operation>();

            foreach (Plant plantElement in Plants)
            {
                PlantsDictionary.Add(plantElement.PlantId, plantElement);

                IEnumerable<Area> areasPlant = await _supervisorMobilityRepository.GetAreasForPlantAsync(plantElement.PlantId);

                if (areasPlant.Count() > 0)
                    foreach (Area areaElement in areasPlant)
                    {
                        AreasDictionary.Add((plantElement.PlantId, areaElement.AreaId), areaElement);

                        IEnumerable<Distribution> distributions = await _supervisorMobilityRepository.GetDistributionsForAreaAsync(areaElement.AreaId);

                        foreach (Distribution distribution in distributions)
                        {
                            DistributionsDictionary.Add((plantElement.PlantId, areaElement.AreaId, distribution.DistributionId), distribution);

                            IEnumerable<Operation> operations = await _supervisorMobilityRepository.GetOperationsForDistributionAsync(distribution.DistributionId);

                            foreach (Operation operation in operations)
                            {
                                OperationsDictionary.Add((plantElement.PlantId, areaElement.AreaId, distribution.DistributionId, operation.OperationId), operation);
                            }

                        }
                    }
            }
            //Fin recoleccinon de datos en bd


            //Verificar que el archivo a cargar, el area corresponde con la del supervisor si es admin hay que realizar la carga sin exepcion


            string MailSubject = "";
            string MailBody = "";



            //Start Massive Upload 
            string filepath = Directory.GetCurrentDirectory().ToString() + "\\uploads\\assycharts\\" + FileNameForStorage;
            try
            {
                using (var workBook = new XLWorkbook(filepath))
                {
                    var pages = workBook.Worksheets.Count;
                    int CountCreateAssycchart = 0;

                    for (int p = 1; p <= pages; p++)
                    {

                        //if (p == 2)
                        //{
                        //    break;
                        //}

                        PathInfo PathResume = new PathInfo();

                        int CountCreateOperation = 0;
                        IXLWorksheet worksheet = workBook.Worksheet(p);

                        //La optencion del producto ira dada por los tiempos

                        //var productCode = ws.Name;
                        //Debug.WriteLine($"Product Name: {productCode}");

                        //var ProductExist = Products.Select(pair => new
                        //{
                        //    Product = pair,
                        //    Similarity = 1 - pair.Code.JaccardDistance(productCode)
                        //}).OrderByDescending(result => result.Similarity).FirstOrDefault();

                        string pageName = worksheet.Name;

                        var CellAreaCode = "B6";
                        var CellDistributionCode = "D6";

                        IXLCell AreaCell = worksheet.Cell(CellAreaCode);
                        IXLCell DistributionCell = worksheet.Cell(CellDistributionCode);

                        var CellStarOperationCode = "B12";


                      
                        var ExcelAreaCode = AreaCell.Value.ToString() != "" ? AreaCell.Value.ToString() : "";

                        //var ExcelAreaDescription = ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 4).Value.ToString() != "" ? ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 4).Value.ToString() : "";

                        var ExcelDistDescription = DistributionCell.Value.ToString() != "" ? DistributionCell.Value.ToString() : "";

                        if (ExcelAreaCode.IsNullOrEmpty() && ExcelDistDescription.IsNullOrEmpty())
                        {
                            break;
                        }


                        // Buscar el ID de planta en el diccionario de plantas
                        var planta = PlantsDictionary.Values.FirstOrDefault(p => p.PlantId == plantId);
                        if (planta != null)
                        {
                            PathResume.PlantId = planta.PlantId;
                        }
                        else
                        {
                            //mensaje de error
                            return;
                        }



                        int maxRetries = 5; // Número máximo de intentos
                        int i = 0; // Número máximo de intentos
                        TimeSpan retryInterval = TimeSpan.FromSeconds(5); // Intervalo de tiempo entre intentos (5 segundos en este caso)
                        int retries = 0;

                        while (retries < maxRetries)
                        {
                            try
                            {

                                if (PathResume.PlantId > 0)
                                {// Buscar coincidencia en area
                                    var coincidenciasAreas = AreasDictionary
                                           .Where(pair => pair.Key.Item1 == PathResume.PlantId) // Filtramos por ID de planta
                                           .Select(pair => new
                                           {
                                               Area = pair.Value,
                                               Similarity = pair.Value.Code.Equals(ExcelAreaCode)
                                                   ? 1.0 // Si los códigos coinciden exactamente, la similitud es máxima
                                                   : 1 - pair.Value.Code.JaccardDistance(ExcelAreaCode)
                                           })
                                           .OrderByDescending(result => result.Similarity)
                                           .FirstOrDefault();

                                    if (coincidenciasAreas != null && coincidenciasAreas.Similarity >= 0.70)
                                    {
                                        PathResume.AreaId = coincidenciasAreas.Area.AreaId;
                                        PathResume.DescripcionArea = coincidenciasAreas.Area.Description;
                                    }


                                    //Renglones de la pagina
                                    var rows = worksheet.Rows();

                                    //rango de los productos
                                    var ranges = new List<IXLRange> {
                                                worksheet.Range("F10:J11"),
                                                worksheet.Range("K10:O11"),
                                                worksheet.Range("P10:T11"),
                                                worksheet.Range("U10:Y11")
                                            };

                                    //Lista de productos que usare en el json
                                    var products = new List<Dictionary<string, Dictionary<string, string>>>();
                                    //Creacion de los 3 productos, con inicializacion
                                    foreach (var range in ranges)
                                    {
                                        var productName = range.FirstRow().FirstCell().Value.ToString();
                                        var nameTime = string.Join("§", range.LastRow().Cells().Select(c => c.Value.ToString()));
                                        var time = "§§§§";
                                        var aditionalTime = "§§§§";
                                        var standarTime = "§§§§";

                                        var product = new Dictionary<string, Dictionary<string, string>>
                                                {
                                                    {
                                                        productName,
                                                        new Dictionary<string, string>
                                                        {
                                                            { "NameTime", nameTime },
                                                            { "Time", time },
                                                            { "aditionalTime", aditionalTime },
                                                            { "standarTime", standarTime }
                                                        }
                                                    }
                                                };

                                        products.Add(product);
                                    }

                                    //Renglon de inicio 
                                    var startingRow = worksheet.Row(12);
                                    //Ciclo para optener las pociones de tiempo estandar y tiempo adicional
                                    int StartAditionalTime = 0;

                                    foreach (var row in rows.SkipWhile(r => r.RowNumber() < startingRow.RowNumber()))
                                    {
                                        // Obtener la celda en la columna B para cada renglón
                                        var cellB = row.Cell("B");
                                        if (cellB.IsMerged() && row.RowNumber() >= 12)
                                        {
                                            StartAditionalTime = row.RowNumber();
                                            break;
                                        }
                                    }

                                    //Aditional Time For product
                                    var aditionalTimeRow = worksheet.Row(StartAditionalTime);
                                    var rangeAditionalTime = worksheet.Range(aditionalTimeRow.Cell("F"), aditionalTimeRow.Cell("Y"));
                                    var cellsAditionalTime = rangeAditionalTime.Cells().ToList();
                                    var aditionalTimeGroups = new List<string>();

                                    for (int j = 0; j < cellsAditionalTime.Count; j += 5)
                                    {
                                        var group = cellsAditionalTime.Skip(j).Take(5).Select(c => c.Value.ToString());
                                        var timeGroup = string.Join("§", group);
                                        aditionalTimeGroups.Add(timeGroup);
                                    }

                                    for (int j = 0; j < products.Count; j++)
                                    {
                                        var product = products[i];
                                        var productName = product.Keys.First();
                                        product[productName]["aditionalTime"] = aditionalTimeGroups[i];
                                    }

                                    //Standar Time For product
                                    var standarTimeRow = worksheet.Row(StartAditionalTime + 1);
                                    var rangeStandarTime = worksheet.Range(standarTimeRow.Cell("F"), standarTimeRow.Cell("Y"));
                                    var cellsStandarTime = rangeStandarTime.Cells().ToList();
                                    var standarTimeGroups = new List<string>();

                                    for (int j = 0; j < cellsStandarTime.Count; j += 5)
                                    {
                                        var group = cellsStandarTime.Skip(j).Take(5).Select(c => c.Value.ToString());
                                        var timeGroup = string.Join("§", group);
                                        standarTimeGroups.Add(timeGroup);
                                    }

                                    for (int j = 0; j < products.Count; j++)
                                    {
                                        var product = products[i];
                                        var productName = product.Keys.First();
                                        product[productName]["standarTime"] = standarTimeGroups[i];
                                    }


                                    if (PathResume.AreaId > 0)
                                    {
                                        // Buscar coincidencia en distribucion
                                        var coincidenciasDistributions = DistributionsDictionary
                                            .Where(pair => pair.Key.Item1 == PathResume.PlantId && pair.Key.Item2 == PathResume.AreaId)
                                            .Select(pair => new
                                            {
                                                Distribution = pair.Value,
                                                Similarity = 1 - pair.Value.Description.JaccardDistance(ExcelDistDescription)
                                            })
                                            .OrderByDescending(result => result.Similarity)
                                            .FirstOrDefault();

                                        if (coincidenciasDistributions != null && coincidenciasDistributions.Similarity > 0.5)
                                        {
                                            PathResume.DistributionId = coincidenciasDistributions.Distribution.DistributionId;
                                            PathResume.DescripcionDistribucion = coincidenciasDistributions.Distribution.Description;
                                        }


                                        if (PathResume.DistributionId > 0)
                                        {


                                            //Optencion de los tiempos por renglon en base a operacion
                                            foreach (var row in rows.SkipWhile(r => r.RowNumber() < startingRow.RowNumber()))
                                            {
                                                // Obtener la celda en la columna B para cada renglón
                                                var cellB = row.Cell("B");

                                                // Verificar si la celda no está combinada y es mayor o igual a la fila 12
                                                if (!cellB.IsMerged() && row.RowNumber() >= 12)
                                                {
                                                    var CellOpCode = row.Cell("C");
                                                    var CellOpDesc = row.Cell("D");
                                                    var CellCommentaryOrRestriction = row.Cell("E");

                                                    var ExcelOpCode = CellOpCode.Value.ToString() != "" ? CellOpCode.Value.ToString() : "";
                                                    var ExcelOpDescription = CellOpDesc.Value.ToString() != "" ? CellOpDesc.Value.ToString() : "";

                                                    var ExcelCommentaryOrRestriction = CellCommentaryOrRestriction.Value.ToString() != "" ? CellCommentaryOrRestriction.Value.ToString() : "";

                                                    if (ExcelOpCode.IsNullOrEmpty() && ExcelOpDescription.IsNullOrEmpty())
                                                    {
                                                        //si es renglon vacio brincamos al siguiente
                                                        continue;
                                                    }

                                                    var coincidenciasOperaciones = OperationsDictionary
                                                   .Where(pair => pair.Key.Item1 == PathResume.PlantId && pair.Key.Item2 == PathResume.AreaId && pair.Key.Item3 == PathResume.DistributionId)
                                                   .Select(pair => new
                                                   {
                                                       Operation = pair.Value,
                                                       Similarity = (pair.Value.Code == ExcelOpCode && pair.Value.Description == ExcelOpDescription ? 1 : 0)
                                                   })
                                                   .OrderByDescending(result => result.Similarity)
                                                   .FirstOrDefault();

                                                    if (coincidenciasOperaciones != null && coincidenciasOperaciones.Similarity > 0.5)
                                                    {
                                                        PathResume.OperationId = coincidenciasOperaciones.Operation.OperationId;
                                                    }


                                                    var range = worksheet.Range(row.Cell("F"), row.Cell("Y"));

                                                    var cells = range.Cells().ToList();
                                                    var timeGroups = new List<string>();

                                                    for (int j = 0; j < cells.Count; j += 5)
                                                    {
                                                        var group = cells.Skip(j).Take(5).Select(c => c.Value.ToString());
                                                        var timeGroup = string.Join("§", group);
                                                        timeGroups.Add(timeGroup);
                                                    }

                                                    for (int j = 0; j < products.Count; j++)
                                                    {
                                                        var product = products[j];
                                                        var productName = product.Keys.First();
                                                        product[productName]["Time"] = timeGroups[j];
                                                    }

                                                    var productsCopy = products.ToList();

                                                    // Eliminar productos sin tiempo de la copia
                                                    productsCopy = productsCopy.Where(product => product.Values.First()["Time"] != "§§§§").ToList();

                                                    // Convertir la copia a JSON string
                                                    var jsonStringCopy = Newtonsoft.Json.JsonConvert.SerializeObject(productsCopy, Newtonsoft.Json.Formatting.Indented);

                                                    //Verificacion de datos....
                                                    //foreach (var product in productsCopy)
                                                    //{
                                                    //    foreach (var entry in product)
                                                    //    {
                                                    //        var productName = entry.Key;
                                                    //        var productData = entry.Value;

                                                    //        var nameTime = productData["NameTime"];
                                                    //        var time = productData["Time"];

                                                    //        Debug.WriteLine($"Nombre del Producto: {productName}");
                                                    //        Debug.WriteLine($"NameTime: {nameTime}");
                                                    //        Debug.WriteLine($"Time: {time}");
                                                    //        Debug.WriteLine("");
                                                    //    }
                                                    //}


                                                    if (PathResume.OperationId > 0)
                                                    {
                                                        Debug.WriteLine($"La Operacion {ExcelOpCode} - {ExcelDistDescription} Existe :) !!! ");
                                                        //Aqui una verificacion de informacion, si algun dato en los tiempos cambia, hay que actualizar el json//
                                                        // Update a la base de datos

                                                        bool isUpdate = false;
                                                        OperationForUpdateDto OperationforUpdate = _mapper.Map<OperationForUpdateDto>(coincidenciasOperaciones.Operation);

                                                        if (OperationforUpdate.jsonTimeProduct != jsonStringCopy)
                                                        {
                                                            OperationforUpdate.jsonTimeProduct = jsonStringCopy;
                                                            isUpdate = true;
                                                        }

                                                        if (OperationforUpdate.restrictionorcomment != ExcelCommentaryOrRestriction)
                                                        {
                                                            OperationforUpdate.jsonTimeProduct = ExcelCommentaryOrRestriction;
                                                            isUpdate = true;

                                                        }

                                                        if (isUpdate)
                                                        {
                                                            var operationEntity = await _assyChartService.FetchOperationAsync((int)PathResume.DistributionId, (int)PathResume.OperationId); ;
                                                            if (operationEntity == null)
                                                            {
                                                                //Dio error aqui va Log
                                                            }

                                                            await _assyChartService.UpdateOperationAsync(OperationforUpdate, operationEntity);
                                                        }

                                                    

                                                    }
                                                    else
                                                    {//No existe hay que crearla
                                                        Debug.WriteLine($"La Operacion {ExcelOpCode} - {ExcelDistDescription} No Existe :c ");



                                                        var operationForCreate = _mapper.Map<OperationForCreationDto>(new OperationForCreationDto() { Code = ExcelOpCode, Description = ExcelOpDescription, IsActive = true });
                                                        operationForCreate.jsonTimeProduct = ExcelCommentaryOrRestriction;
                                                        operationForCreate.jsonTimeProduct = jsonStringCopy;

                                                        var finalOperation = _mapper.Map<Operation>(operationForCreate);
                                                        await _supervisorMobilityRepository.AddOperationForDistributionAsync((int)PathResume.AreaId, (int)PathResume.DistributionId, finalOperation);
                                                        await _supervisorMobilityRepository.SaveChangesAsync();

                                                        OperationsDictionary.Add(((int)PathResume.PlantId, (int)PathResume.AreaId, (int)PathResume.DistributionId, finalOperation.OperationId), finalOperation);
                                                        CountCreateOperation++;
                                                    }

                                                }
                                                else if (cellB.IsMerged() && row.RowNumber() >= 12)
                                                {
                                                    //Finalizamos recorrido de renglones, ya no hay mas operaciones
                                                    break;
                                                }
                                            }




                                        }//end if distribution >0
                                        else
                                        {
                                            ////distribution no existe- se crea todo
                                            string codeGen = ExcelDistDescription;

                                            SlugHelper slugHelper = new SlugHelper();
                                            string slug = slugHelper.GenerateSlug(codeGen);

                                            var distributionForCreate = _mapper.Map<DistributionForCreationDto>(new DistributionForCreationDto() { Code = slug, Description = ExcelDistDescription, IsActive = true });
                                            var finalDistribution = _mapper.Map<Distribution>(distributionForCreate);
                                            await _supervisorMobilityRepository.AddDistributionForPlantAsync((int)PathResume.PlantId, (int)PathResume.AreaId, finalDistribution);
                                            await _supervisorMobilityRepository.SaveChangesAsync();
                                            PathResume.DistributionId = finalDistribution.DistributionId;
                                            DistributionsDictionary.Add(((int)PathResume.PlantId, (int)PathResume.AreaId, (int)PathResume.DistributionId), finalDistribution);

                                            await _supervisorMobilityRepository.AddDistributionForProductAsync((int)PathResume.ProductID, finalDistribution);

                                            // aqui va el ciclo de operacion
                                            //Optencion de los tiempos por renglon en base a operacion
                                           
                                        }//end else distribuccion no existe

                                    }//end if area > 0
                                    else
                                    {
                                        SlugHelper slugHelper = new SlugHelper(); 

                                        ////area no existe- se crea todo
                                        var areaForCreate = _mapper.Map<AreaForCreationDto>(new AreaForCreationDto() { Code = ExcelAreaCode, Description = ExcelAreaCode, IsActive = true });

                                        var finalArea = _mapper.Map<Area>(areaForCreate);
                                        finalArea.PlantId = (int)PathResume.PlantId;

                                        await _supervisorMobilityRepository.AddArea(finalArea);
                                        await _supervisorMobilityRepository.AddAreaForPlantAsync((int)PathResume.PlantId, finalArea);
                                        await _supervisorMobilityRepository.SaveChangesAsync();
                                        PathResume.AreaId = finalArea.AreaId;

                                        AreasDictionary.Add(((int)PathResume.PlantId, (int)PathResume.AreaId), finalArea);

                                        ////la distribuccion no existira
                                        string codeGen = ExcelDistDescription;

                                        var distributionForCreate = _mapper.Map<DistributionForCreationDto>(new DistributionForCreationDto() { Code = codeGen, Description = ExcelDistDescription, IsActive = true });
                                        var finalDistribution = _mapper.Map<Distribution>(distributionForCreate);
                                        await _supervisorMobilityRepository.AddDistributionForPlantAsync((int)PathResume.PlantId, (int)PathResume.AreaId, finalDistribution);
                                        await _supervisorMobilityRepository.SaveChangesAsync();
                                        PathResume.DistributionId = finalDistribution.DistributionId;

                                        DistributionsDictionary.Add(((int)PathResume.PlantId, (int)PathResume.AreaId, (int)PathResume.DistributionId), finalDistribution);
                                        await _supervisorMobilityRepository.AddDistributionForProductAsync((int)PathResume.ProductID, finalDistribution);

                                        ////la operacion no existira


                                        foreach (var row in rows.SkipWhile(r => r.RowNumber() < startingRow.RowNumber()))
                                        {
                                            // Obtener la celda en la columna B para cada renglón
                                            var cellB = row.Cell("B");

                                            // Verificar si la celda no está combinada y es mayor o igual a la fila 12
                                            if (!cellB.IsMerged() && row.RowNumber() >= 12)
                                            {
                                                var CellOpCode = row.Cell("C");
                                                var CellOpDesc = row.Cell("D");
                                                var CellCommentaryOrRestriction = row.Cell("E");

                                                var ExcelOpCode = CellOpCode.Value.ToString() != "" ? CellOpCode.Value.ToString() : "";
                                                var ExcelOpDescription = CellOpDesc.Value.ToString() != "" ? CellOpDesc.Value.ToString() : "";

                                                var ExcelCommentaryOrRestriction = CellCommentaryOrRestriction.Value.ToString() != "" ? CellCommentaryOrRestriction.Value.ToString() : "";

                                                if (ExcelOpCode.IsNullOrEmpty() && ExcelOpDescription.IsNullOrEmpty())
                                                {
                                                    //si es renglon vacio brincamos al siguiente
                                                    continue;
                                                }

                                                var range = worksheet.Range(row.Cell("F"), row.Cell("Y"));

                                                var cells = range.Cells().ToList();
                                                var timeGroups = new List<string>();

                                                for (int j = 0; j < cells.Count; j += 5)
                                                {
                                                    var group = cells.Skip(j).Take(5).Select(c => c.Value.ToString());
                                                    var timeGroup = string.Join("§", group);
                                                    timeGroups.Add(timeGroup);
                                                }

                                                for (int j = 0; j < products.Count; j++)
                                                {
                                                    var product = products[j];
                                                    var productName = product.Keys.First();
                                                    product[productName]["Time"] = timeGroups[j];
                                                }

                                                var productsCopy = products.ToList();

                                                // Eliminar productos sin tiempo de la copia
                                                productsCopy = productsCopy.Where(product => product.Values.First()["Time"] != "§§§§").ToList();

                                                // Convertir la copia a JSON string
                                                var jsonStringCopy = Newtonsoft.Json.JsonConvert.SerializeObject(productsCopy, Newtonsoft.Json.Formatting.Indented);

                                                //Verificacion de datos....
                                                //foreach (var product in productsCopy)
                                                //{
                                                //    foreach (var entry in product)
                                                //    {
                                                //        var productName = entry.Key;
                                                //        var productData = entry.Value;

                                                //        var nameTime = productData["NameTime"];
                                                //        var time = productData["Time"];

                                                //        Debug.WriteLine($"Nombre del Producto: {productName}");
                                                //        Debug.WriteLine($"NameTime: {nameTime}");
                                                //        Debug.WriteLine($"Time: {time}");
                                                //        Debug.WriteLine("");
                                                //    }
                                                //}


                                                //No existe hay que crearla
                                                Debug.WriteLine($"La Operacion {ExcelOpCode} - {ExcelDistDescription} No Existe :c ");



                                                var operationForCreate = _mapper.Map<OperationForCreationDto>(new OperationForCreationDto() { Code = ExcelOpCode, Description = ExcelOpDescription, IsActive = true });
                                                operationForCreate.jsonTimeProduct = ExcelCommentaryOrRestriction;
                                                operationForCreate.jsonTimeProduct = jsonStringCopy;

                                                var finalOperation = _mapper.Map<Operation>(operationForCreate);
                                                await _supervisorMobilityRepository.AddOperationForDistributionAsync((int)PathResume.AreaId, (int)PathResume.DistributionId, finalOperation);
                                                await _supervisorMobilityRepository.SaveChangesAsync();

                                                OperationsDictionary.Add(((int)PathResume.PlantId, (int)PathResume.AreaId, (int)PathResume.DistributionId, finalOperation.OperationId), finalOperation);
                                                CountCreateOperation++;

                                            }

                                        }

                                    }

                                }//end if plant >0
                                 // no hay chance de que la planta no exista


                                var AssyChartExist = await _supervisorMobilityRepository.GetAssyChartForJobObservationAsync((int)PathResume.PlantId, (int)PathResume.AreaId, (int)PathResume.DistributionId);

                                if (AssyChartExist is null)
                                {
                                    AssyChartForCreation assychartForCreate = new AssyChartForCreation()
                                    {
                                        PlantId = (int)PathResume.PlantId,
                                        AreaId = (int)PathResume.AreaId,
                                        DistributionId = (int)PathResume.DistributionId,
                                        CreationDate = DateTime.Now,
                                        ModificationDate = DateTime.Now,
                                        IsActive = true
                                    };

                                    var resultCreateAssy = await _assyChartService.CreateAssyChartAsync(assychartForCreate);
                                    CountCreateAssycchart++;
                                    if (resultCreateAssy != null)
                                    {
                                        //se crea assy chart cout
                                        Debug.WriteLine($"Create assychart id {resultCreateAssy.AssyChardId} plantid {(int)PathResume.PlantId} areaid {(int)PathResume.AreaId} distributionid {(int)PathResume.DistributionId} ");
                                    }
                                }

                                retries = 0;
                                // Si la operación tiene éxito, puedes salir del bucle
                                break;
                            }
                            catch (Exception ex)
                            {
                                // Maneja la excepción aquí, si es necesario
                                Debug.WriteLine($"I Value:{i}");
                                Debug.WriteLine($"Intento {retries + 1} falló: {ex.Message}");

                                // Incrementa el número de intentos
                                retries++;

                                // Espera el intervalo de tiempo antes de volver a intentarlo
                                await Task.Delay(retryInterval);
                            }



                        }//end While retries



                        Debug.WriteLine($"Pagina {p} : {pageName} ");

                    }//for de paginas

                    Debug.WriteLine($"AssyTotal {CountCreateAssycchart} ");
                }//end using



            }//end try
            catch (Exception ex)
            {
                Debug.WriteLine($"Error Tree Data: {ex.Message.ToString()}");
            }//end trycatch to add excel to list


            int maxRetriesMail = 5; // Número máximo de intentos
            TimeSpan retryIntervalMail = TimeSpan.FromSeconds(5); // Intervalo de tiempo entre intentos (5 segundos en este caso)
            int retriesMail = 0;

            while (retriesMail < maxRetriesMail)
            {
                try
                {
                    //var emailMessage = _email.CreateEmailMessage(userEntity.Email, "Plant Structure document has been processed, you can now review its contents on the Paths details page.");
                    //_email.Send(emailMessage);
                    Debug.WriteLine($"Correo Enviado TreeData");
                    break;
                }
                catch (Exception ex)
                {

                    // Maneja la excepción aquí, si es necesario
                    Debug.WriteLine($"Fallo Enviar mail: {ex.Message}");

                    // Incrementa el número de intentos
                    retriesMail++;

                    // Espera el intervalo de tiempo antes de volver a intentarlo
                    await Task.Delay(retryIntervalMail);
                }

            }

            // notificacion
            //añade notificacion de error

            int maxIntentos = 5; // Número máximo de intentos
            TimeSpan newintentTime = TimeSpan.FromSeconds(5); // Intervalo de tiempo entre intentos (5 segundos en este caso)
            int intentos = 0;

            while (intentos < maxIntentos)
            {
                try
                {

                    Notification NotyFinish = new Notification();
                    NotyFinish.NotificationType = $"PlantStructureData Procces {DateTime.Now}";
                    NotyFinish.NotificationText = $"PlantStructureData document has been processed, you can now review its contents on the Plant Details details page.";

                    NotyFinish.MadeBy = "PlantStructureData Process System ";
                    NotyFinish.UserId = userEntity.UserId;
                    NotyFinish.IsAccepted = true;
                    NotyFinish.IsActive = true;
                    NotyFinish.EntryDate = DateTime.Now;

                    _supervisorMobilityRepository.AddNotificationAsync(NotyFinish);
                    break;
                }
                catch (Exception ex)
                {

                    // Maneja la excepción aquí, si es necesario
                    Debug.WriteLine($"Fallo crear Succes Notification: {ex.Message}");

                    // Incrementa el número de intentos
                    intentos++;
                    if (intentos == 5)
                    {
                        //añade notificacion de error
                        Notification NotyError = new Notification();
                        NotyError.NotificationType = $"PlantStructureData Procces Finish: {DateTime.Now}";
                        NotyError.NotificationText = $"PlantStructureData procces document";

                        NotyError.MadeBy = "PlantStructureData System";
                        NotyError.UserId = userEntity.UserId;
                        NotyError.IsAccepted = true;
                        NotyError.IsActive = true;
                        NotyError.EntryDate = DateTime.Now;
                        _supervisorMobilityRepository.AddNotificationAsync(NotyError);
                    }


                    // Espera el intervalo de tiempo antes de volver a intentarlo
                    await Task.Delay(newintentTime);
                }

            }
            await _supervisorMobilityRepository.SaveChangesAsync();


        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AreaWithJustOperationsDto>>> GetAreas(
                    int plantId, bool includeCollections = false)
        {
            if (!await _supervisorMobilityRepository.PlantExistAsync(plantId))
            {
                return NotFound();
            }

            if (includeCollections)
            {
                var areasForPlantWhitDistributions = await _supervisorMobilityRepository.GetAreasForPlantAsync(plantId, includeCollections);
                return Ok(_mapper.Map<IEnumerable<AreaWithJustOperationsDto>>(areasForPlantWhitDistributions));

            }
            else
            {
                var areasForPlant = await _supervisorMobilityRepository
                                .GetAreasForPlantAsync(plantId);
                return Ok(_mapper.Map<IEnumerable<AreaWithoutNavigationPropertiesDto>>(areasForPlant));

            }


        }

        [HttpGet("{areaId}", Name = "GetArea")]
        public async Task<ActionResult<AreaWithoutNavigationPropertiesDto>> GetArea(
           int plantId, int areaId, bool includeOperations = false)
        {
            if (!await _supervisorMobilityRepository.PlantExistAsync(plantId))
            {
                return NotFound();
            }

            var area = await _supervisorMobilityRepository
                .GetAreaForPlantAsync(plantId, areaId, includeOperations);

            if (area == null)
            {
                return NotFound();
            }

            if (includeOperations)
            {
                return Ok(_mapper.Map<AreaWithJustOperationsDto>(area));
            }
            return Ok(_mapper.Map<AreaWithoutNavigationPropertiesDto>(area));
        }

        [HttpPost]
        public async Task<ActionResult<AreaWithoutNavigationPropertiesDto>> CreateArea(
            int plantId,
            AreaForCreationDto area)
        {
            if (!await _supervisorMobilityRepository.PlantExistAsync(plantId))
            {
                return NotFound();
            }

            var finalArea = _mapper.Map<Area>(area);
            finalArea.PlantId = plantId;

            await _supervisorMobilityRepository.AddArea(finalArea);


            await _supervisorMobilityRepository.AddAreaForPlantAsync(
                plantId, finalArea);

            await _supervisorMobilityRepository.SaveChangesAsync();

            var createdAreaToReturn =
                _mapper.Map<AreaWithoutNavigationPropertiesDto>(finalArea);

            return CreatedAtRoute("GetArea",
                new
                {
                    plantId,
                    areaId = createdAreaToReturn.AreaId
                },
                createdAreaToReturn);
        }

        [HttpPut("{areaid}")]
        public async Task<ActionResult> UpdateArea(int plantId, int areaId,
            AreaForUpdateDto area)
        {
            if (!await _supervisorMobilityRepository.PlantExistAsync(plantId))
            {
                return NotFound();
            }

            var areaEntity = await _supervisorMobilityRepository
                .GetAreaForPlantAsync(plantId, areaId);
            if (areaEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(area, areaEntity);

            await _supervisorMobilityRepository.SaveChangesAsync();

            return Ok();
        }

        [HttpPatch("{areaid}")]
        public async Task<ActionResult> PartiallyUpdateArea(
            int plantId, int areaId,
            JsonPatchDocument<AreaForUpdateDto> patchDocumentArea)
        {
            if (!await _supervisorMobilityRepository.PlantExistAsync(plantId))
            {
                return NotFound();
            }

            var areaEntity = await _supervisorMobilityRepository
                .GetAreaForPlantAsync(plantId, areaId);
            if (areaEntity == null)
            {
                return NotFound();
            }

            var areaToPatch = _mapper.Map<AreaForUpdateDto>(areaEntity);

            patchDocumentArea.ApplyTo(areaToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(areaToPatch))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(areaToPatch, areaEntity);

            await _supervisorMobilityRepository.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{areaId}")]
        public async Task<ActionResult> DeleteArea(int plantId, int areaId)
        {
            if (!await _supervisorMobilityRepository.PlantExistAsync(plantId))
            {
                return NotFound();
            }

            var areaEntity = await _supervisorMobilityRepository
                .GetAreaForPlantAsync(plantId, areaId);
            if (areaEntity == null)
            {
                return NotFound();
            }

            _supervisorMobilityRepository.DeleteArea(areaEntity);
            await _supervisorMobilityRepository.SaveChangesAsync();

            return Ok();
        }
    }
}
