using AutoMapper;
using ClosedXML.Excel;
using DuoVia.FuzzyStrings;
using FuzzyString;
using Microsoft.EntityFrameworkCore;
using Slugify;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.Paths;
using SupervisorMobility.API.DataAccess.Entities.TreeStruct;
using SupervisorMobility.API.DataAccess.Services.TreeServices;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Entities.CDMS;
using SupervisorMobility.API.Models.AreaDtos;
using SupervisorMobility.API.Models.AssyChart;
using SupervisorMobility.API.Models.DistributionDtos;
using SupervisorMobility.API.Models.OperationDtos;
using SupervisorMobility.API.Services;
using System.Diagnostics;

namespace SupervisorMobility.API.DataAccess.Services.BackgroundProcessServices
{
    public class ProcessWorkLoadService
    {
        private readonly IServiceProvider _serviceProvider;

        public ProcessWorkLoadService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task ProcessWorkLoadDataAsync(string trustedFileNameForStorage, int plantnameid, int UserIdUpload, CancellationToken stoppingToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;

                var _supervisorMobilityRepository = serviceProvider.GetRequiredService<ISupervisorMobilityRepository>();
                var _assyChartService = serviceProvider.GetRequiredService<IAssyChartService>();
                var _email = serviceProvider.GetRequiredService<IEmailService>();
                var _treeService = serviceProvider.GetRequiredService<ITreeService>();
                var customHttp = serviceProvider.GetRequiredService<CustomHttpClientService>();
                var _bridgeHttpClient = customHttp.GetBridgeHttpClient();
                var _mapper = serviceProvider.GetRequiredService<IMapper>();
                var _env = serviceProvider.GetRequiredService<IWebHostEnvironment>();
                var _context = serviceProvider.GetRequiredService<SupervisorMobilityContext>();

                User userEntity = await _supervisorMobilityRepository.GetUserAsync(UserIdUpload, false);

                string eMailSubject = "";
                string eMailBody = "PlantStructureData document has been processed.";

                bool DocumentError = false;

                using (var dbContext = _context)
                {
                    using (var transaction = dbContext.Database.BeginTransaction())
                    {
                        try
                        {


                            IEnumerable<Plant> Plants = await _context.Plants.Where(u => u.IsActive == true).OrderBy(c => c.PlantId).ToListAsync();
                            IEnumerable<Product> Products = await _context.Products.OrderBy(c => c.ProductId).Include(p => p.Distributions).ToListAsync();

                            Dictionary<int, Plant> PlantsDictionary = new Dictionary<int, Plant>();
                            Dictionary<(int, int), Area> AreasDictionary = new Dictionary<(int, int), Area>();
                            Dictionary<(int, int, int), Distribution> DistributionsDictionary = new Dictionary<(int, int, int), Distribution>();
                            Dictionary<(int, int, int, int), Operation> OperationsDictionary = new Dictionary<(int, int, int, int), Operation>();

                            foreach (Plant plantElement in Plants)
                            {
                                PlantsDictionary.Add(plantElement.PlantId, plantElement);

                                IEnumerable<Area> areasPlant = await _context.Areas.Where(a => a.PlantId == plantElement.PlantId && a.IsActive == true).ToListAsync();

                                if (areasPlant.Count() > 0)
                                    foreach (Area areaElement in areasPlant)
                                    {
                                        AreasDictionary.Add((plantElement.PlantId, areaElement.AreaId), areaElement);

                                        IEnumerable<Distribution> distributions = await _context.Distributions.Where(o => o.AreaId == areaElement.AreaId && o.IsActive == true).ToListAsync();

                                        foreach (Distribution distribution in distributions)
                                        {
                                            DistributionsDictionary.Add((plantElement.PlantId, areaElement.AreaId, distribution.DistributionId), distribution);

                                            IEnumerable<Operation> operations = await _context.Operations.Where(o => o.DistributionId == distribution.DistributionId && o.IsActive == true).ToListAsync();
                                            foreach (Operation operation in operations)
                                            {
                                                OperationsDictionary.Add((plantElement.PlantId, areaElement.AreaId, distribution.DistributionId, operation.OperationId), operation);
                                            }

                                        }
                                    }
                            }
                            //Fin recoleccinon de datos en bd

                            //Get Rutas CDMS
                            CDMS_GOS_Directory GOSFolders = new CDMS_GOS_Directory();
                            TreeItemData rootNodeGOS = new TreeItemData();

                            CDMS_CCP_Directory CCPFolders = new CDMS_CCP_Directory();
                            TreeItemData rootNodeCCP = new TreeItemData();

                            CDMS_HOE_Directory HOEFolders = new CDMS_HOE_Directory();
                            TreeItemData rootNodeHOE = new TreeItemData();

                            //Obtencion de rutas y creacion de arboles de carpetas
                            try
                            {
                                //Recoleccion de rutas de GOS
                                try
                                {
                                    try
                                    {
                                        var response = await _bridgeHttpClient.GetAsync("SMGOS/GetDirectoryPathsGOS");

                                        if (response.IsSuccessStatusCode)
                                        {
                                            var result = await response.Content.ReadFromJsonAsync<CDMS_GOS_Directory>();
                                            GOSFolders = result;
                                        }
                                        else
                                        {
                                            Console.WriteLine($"GET FOLDERS GOS, Status Code {response.StatusCode} : {response.Content.ReadAsStringAsync().Result}");
                                        }
                                    }
                                    catch (HttpRequestException ex)
                                    {
                                        Console.WriteLine($"Error al hacer la solicitud: {ex.Message}");
                                    }
                                    catch (TaskCanceledException ex)
                                    {
                                        Console.WriteLine($"La solicitud ha sido cancelada: {ex.Message}");
                                    }

                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Error Get GOS Folder From CDMS");
                                    Console.WriteLine(ex.Message);
                                }

                                //Construccion de arbol Gos
                                if (GOSFolders != null)
                                {
                                    rootNodeGOS = _treeService.ConstruirArbolGOS(GOSFolders.operation);
                                }
                                //Recoleccion de rutas de CCP
                                try
                                {
                                    try
                                    {
                                        var response = await _bridgeHttpClient.GetAsync("SMCCP/GetDirectoryPathsCCP");

                                        if (response.IsSuccessStatusCode)
                                        {
                                            var result = await response.Content.ReadFromJsonAsync<CDMS_CCP_Directory>();
                                            CCPFolders = result;
                                        }
                                        else
                                        {
                                            //await _js.InvokeVoidAsync("alert", $"Error get folders: {response.Content.ReadAsStringAsync().Result}");
                                            Console.WriteLine($"GET FOLDERS CCP, Status Code {response.StatusCode} : {response.Content.ReadAsStringAsync().Result}");
                                        }
                                    }
                                    catch (HttpRequestException ex)
                                    {
                                        Console.WriteLine($"Error al hacer la solicitud: {ex.Message}");
                                    }
                                    catch (TaskCanceledException ex)
                                    {
                                        Console.WriteLine($"La solicitud ha sido cancelada: {ex.Message}");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Error Get CCP Folder From CCP");
                                    Console.WriteLine(ex.Message);
                                    Console.WriteLine(ex.Message);
                                }
                                //Construccion de arbol CCP
                                if (CCPFolders != null)
                                {
                                    rootNodeCCP = _treeService.ConstruirArbolCCP(CCPFolders.operation);
                                }
                                //Recoleccion de ruta HOE
                                try
                                {
                                    try
                                    {
                                        var response = await _bridgeHttpClient.GetAsync("SMHOE/GetDirectoryPathsHOE");

                                        if (response.IsSuccessStatusCode)
                                        {
                                            var result = await response.Content.ReadFromJsonAsync<CDMS_HOE_Directory>();
                                            HOEFolders = result;
                                        }
                                        else
                                        {
                                            //await _js.InvokeVoidAsync("alert", $"Error get folders: {response.Content.ReadAsStringAsync().Result}");
                                            Console.WriteLine($"GET FOLDERS HOE, Status Code {response.StatusCode} : {response.Content.ReadAsStringAsync().Result}");
                                        }
                                    }
                                    catch (HttpRequestException ex)
                                    {
                                        Console.WriteLine($"Error al hacer la solicitud: {ex.Message}");
                                    }
                                    catch (TaskCanceledException ex)
                                    {
                                        Console.WriteLine($"La solicitud ha sido cancelada: {ex.Message}");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Error Get HOE Folder From CDMS");
                                    Console.WriteLine(ex.Message);
                                }
                                //creacion arbol HOE
                                if (HOEFolders != null)
                                {
                                    rootNodeHOE = _treeService.ConstruirArbolHOE(HOEFolders.operation);
                                }

                            }
                            catch (Exception ex)
                            {
                                //Log de error en alguna carga general
                            }



                            //Start Massive Upload 
                            string filepath = Directory.GetCurrentDirectory().ToString() + "\\uploads\\massive\\" + trustedFileNameForStorage;
                            try
                            {
                                using (var workBook = new XLWorkbook(filepath))
                                {
                                    var pages = workBook.Worksheets.Count;
                                    int CountCreateAssycchart = 0;

                                    for (int p = 1; p <= pages; p++)
                                    {
                                        PathInfo PathResume = new PathInfo();

                                        int CountCreateOperation = 0;
                                        IXLWorksheet worksheet = workBook.Worksheet(p);


                                        string pageName = worksheet.Name;

                                        var CellAreaCode = "B6";
                                        var CellDistributionCode = "D6";

                                        IXLCell AreaCell = worksheet.Cell(CellAreaCode);
                                        IXLCell DistributionCell = worksheet.Cell(CellDistributionCode);


                                        var ExcelAreaCode = AreaCell.Value.ToString() != "" ? AreaCell.Value.ToString() : "";
                                        var ExcelDistDescription = DistributionCell.Value.ToString() != "" ? DistributionCell.Value.ToString() : "";
                                     

                                        if (string.IsNullOrEmpty(ExcelAreaCode) && string.IsNullOrEmpty(ExcelDistDescription))
                                        {
                                            break;
                                        }


                                        var planta = PlantsDictionary.Values.FirstOrDefault(p => p.PlantId == plantnameid);
                                        if (planta != null)
                                        {
                                            PathResume.PlantId = planta.PlantId;
                                        }
                                        else
                                        {
                                            //mensaje de error
                                            return;
                                        }
                                        //buscar Area coincidencia en Planta
                                        var coincidenciasAreas = AreasDictionary
                                                          .Where(pair => pair.Key.Item1 == PathResume.PlantId).Select(pair => new
                                                          {
                                                              Area = pair.Value,
                                                              Similarity = _treeService.CombineSimilaritiesArea(pair.Value.Code, pair.Value.Description, ExcelAreaCode)
                                                          })
                                                          .OrderByDescending(result => result.Similarity).FirstOrDefault();

                                        if (coincidenciasAreas != null && coincidenciasAreas.Similarity >= 0.70)
                                        {
                                            PathResume.AreaId = coincidenciasAreas.Area.AreaId;
                                            PathResume.DescripcionArea = coincidenciasAreas.Area.Description;
                                        }

                                        // Buscar distribucion coincidencia en Area
                                        var coincidenciasDistributions = DistributionsDictionary
                                            .Where(pair => pair.Key.Item1 == PathResume.PlantId && pair.Key.Item2 == PathResume.AreaId)
                                            .Select(pair => new
                                            {
                                                Distribution = pair.Value,
                                                // Se combina la similitud a nivel de caracteres y palabras
                                                Similarity = _treeService.CombineSimilarities(pair.Value.Description, ExcelDistDescription)
                                            })
                                            .OrderByDescending(result => result.Similarity)
                                            .FirstOrDefault();

                                        if (coincidenciasDistributions != null && coincidenciasDistributions.Similarity > 0.95)
                                        {
                                            PathResume.DistributionId = coincidenciasDistributions.Distribution.DistributionId;
                                            PathResume.DescripcionDistribucion = coincidenciasDistributions.Distribution.Description;
                                        }


                                        //Optenemos AssyChart para rutas
                                        AssyChart? AssyChartExist = null;

                                        if (PathResume.PlantId != null && PathResume.AreaId != null && PathResume.DistributionId != null)
                                        {
                                            AssyChartExist = await _context.AssyCharts.Include(pr => pr.RoutesProductsAssyChart).ThenInclude(r => r.Product).Where(p => p.PlantId == (int)PathResume.PlantId && p.AreaId == (int)PathResume.AreaId && p.DistributionId == (int)PathResume.DistributionId).FirstOrDefaultAsync();
                                        }


                                        //si existe no deberia haber problema seria un caso donde la planta existe, el area existe, la distribuccion existe
                                        if (AssyChartExist is null && PathResume.PlantId != null && PathResume.AreaId != null && PathResume.DistributionId != null)
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

                                            //aqui va la transiction 
                                            var finalasssychart = _mapper.Map<AssyChart>(assychartForCreate);
                                            dbContext.AssyCharts.Add(finalasssychart);
                                            dbContext.SaveChanges();
                                            Debug.WriteLine($"dbContext SaveChanges Succesfull ");

                                            CountCreateAssycchart++;

                                            Debug.WriteLine($"Create assychart id {finalasssychart.AssyChardId} plantid {(int)PathResume.PlantId} areaid {(int)PathResume.AreaId} distributionid {(int)PathResume.DistributionId} ");

                                            if (PathResume.PlantId != null && PathResume.AreaId != null && PathResume.DistributionId != null)
                                            {
                                                AssyChartExist = await _context.AssyCharts.Include(pr => pr.RoutesProductsAssyChart).ThenInclude(r => r.Product).Where(p => p.PlantId == (int)PathResume.PlantId && p.AreaId == (int)PathResume.AreaId && p.DistributionId == (int)PathResume.DistributionId).FirstOrDefaultAsync();
                                            }
                                        }


                                        if (PathResume.PlantId > 0)
                                        {
                                            //la planta existe
                                            int rowStartOperation = 12;

                                            var rows = worksheet.Rows();

                                            var possibleRows = new List<int> { 8, 9, 10, 11, 12 };

                                            var products = new List<Dictionary<string, Dictionary<string, string>>>();

                                            foreach (int row in possibleRows)
                                            {

                                                var ranges = new List<IXLRange> {
                                                    worksheet.Range($"F{row}:J{row + 1}"),
                                                    worksheet.Range($"K{row}:O{row + 1}"),
                                                    worksheet.Range($"P{row}:T{row + 1}"),
                                                    worksheet.Range($"U{row}:Y{row + 1}")
                                                };

                                                var productNameVerifyRange = ranges.First().FirstRow().FirstCell().Value.ToString();

                                                var ProductExist = Products.Select(pair => new
                                                {
                                                    Product = pair,
                                                    Similarity = 1 - pair.Code.JaccardDistance(productNameVerifyRange)
                                                }).OrderByDescending(result => result.Similarity).FirstOrDefault();

                                                //Si el rango es correcto deberia coincidir con un producto
                                                if (ProductExist != null && ProductExist.Similarity > 0.5)
                                                {
                                                    //asigno el inicio del documento
                                                    rowStartOperation = row + 2;
                                                    foreach (var range in ranges)
                                                    {
                                                        if (range.IsMerged())
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
                                                                                    { "AdditionalTime", aditionalTime },
                                                                                    { "StandardTime", standarTime }
                                                                                }
                                                                            }
                                                                        };

                                                            products.Add(product);
                                                        }
                                                    }

                                                    if (products.Count > 0)
                                                    {
                                                        break;
                                                    }
                                                }

                                            }

                                            var startingRow = worksheet.Row(rowStartOperation);
                                            int StartAdditionalTime = 0;

                                            foreach (var row in rows.SkipWhile(r => r.RowNumber() < startingRow.RowNumber()))
                                            {
                                                var cellB = row.Cell("B");
                                                if (cellB.IsMerged() && row.RowNumber() >= rowStartOperation)
                                                {
                                                    StartAdditionalTime = row.RowNumber();
                                                    break;
                                                }
                                            }

                                            //Aditional Time For product
                                            var additionalTimeRow = worksheet.Row(StartAdditionalTime);
                                            var rangeAditionalTime = worksheet.Range(additionalTimeRow.Cell("F"), additionalTimeRow.Cell("Y"));
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
                                                var product = products[j];
                                                var productName = product.Keys.First();
                                                product[productName]["AdditionalTime"] = aditionalTimeGroups[j];
                                            }

                                            //Standar Time For product
                                            var standarTimeRow = worksheet.Row(StartAdditionalTime + 1);
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
                                                var product = products[j];
                                                var productName = product.Keys.First();
                                                product[productName]["StandardTime"] = standarTimeGroups[j];
                                            }


                                            if (PathResume.AreaId > 0)
                                            {
                                                //el area existe
                                                if (PathResume.DistributionId > 0)
                                                {
                                                    //la distribucion exsite
                                                    string lastOperationName = null; // Para almacenar el último nombre de operación

                                                    foreach (var row in rows.SkipWhile(r => r.RowNumber() < startingRow.RowNumber()))
                                                    {
                                                        PathResume.OperationId = null;
                                                        // Obtener la celda en la columna B para cada renglón
                                                        var cellB = row.Cell("B");

                                                        // Verificar si la celda no está combinada y es mayor o igual a la fila 12
                                                        if (!cellB.IsMerged() && row.RowNumber() >= rowStartOperation)
                                                        {
                                                            bool allCellsAreEmpty = true;
                                                            var CellOpCode = row.Cell("C");
                                                            var CellOpDesc = row.Cell("D");
                                                            var CellCommentaryOrRestriction = row.Cell("E");

                                                            var ExcelOpCode = CellOpCode.Value.ToString() != "" ? CellOpCode.Value.ToString() : "";

                                                            var ExcelOpDescription = CellOpDesc.Value.ToString() != "" ? CellOpDesc.Value.ToString() : "";

                                                            if (!string.IsNullOrEmpty(ExcelOpDescription))
                                                            {
                                                                lastOperationName = ExcelOpDescription;
                                                            }

                                                            // Si no hay un nombre en la columna B, reutilizamos el último nombre de operación
                                                            if (lastOperationName != null)
                                                            {
                                                                ExcelOpDescription = lastOperationName;
                                                            }

                                                            var ExcelCommentaryOrRestriction = CellCommentaryOrRestriction.Value.ToString() != "" ? CellCommentaryOrRestriction.Value.ToString() : "";

                                                            for (char col = 'C'; col <= 'Y'; col++)
                                                            {
                                                                var cell = row.Cell(col.ToString());
                                                                if (!cell.IsEmpty())
                                                                {
                                                                    allCellsAreEmpty = false;
                                                                    break;
                                                                }
                                                            }

                                                            if (allCellsAreEmpty)
                                                            {
                                                                //si es renglon vacio brincamos al siguiente
                                                                continue;
                                                            }
                                                            else if (string.IsNullOrEmpty(ExcelOpCode) && !string.IsNullOrEmpty(ExcelOpDescription))
                                                            {
                                                                DocumentError = true;
                                                                eMailBody += $"\\n Falta No. Operacion..." +
                                                                    $" Rango de celdas C{row.RowNumber()}" +
                                                                    $" Pagina: {p} - {pageName}" +
                                                                    $" Distribucion: {coincidenciasDistributions.Distribution.Description}";
                                                            }
                                                            else if (!string.IsNullOrEmpty(ExcelOpCode) && string.IsNullOrEmpty(ExcelOpDescription))
                                                            {
                                                                DocumentError = true;
                                                                eMailBody += $"\\n Falta Nombre de operacion..." +
                                                                    $" Rango de celdas C{row.RowNumber()}" +
                                                                    $" Pagina: {p} - {pageName}" +
                                                                    $" Distribucion: {coincidenciasDistributions.Distribution.Description}";
                                                            }

                                                            var range = worksheet.Range(row.Cell("F"), row.Cell("Y"));

                                                            var cells = range.Cells().ToList();
                                                            var timeGroups = new List<string>();

                                                            //se optienen los grupos de tiempos
                                                            for (int j = 0; j < cells.Count; j += 5)
                                                            {
                                                                var group = cells.Skip(j).Take(5).Select(c => c.Value.ToString());
                                                                var timeGroup = string.Join("§", group);
                                                                timeGroups.Add(timeGroup);
                                                            }

                                                            //una copia de los productos vacios para añádir los tiempso correspodnientes
                                                            List<Dictionary<string, Dictionary<string, string>>> productsCopy = ObjectCloner.ObjectCloner.DeepClone(products);
                                                            for (int j = 0; j < productsCopy.Count; j++)
                                                            {
                                                                var product = productsCopy[j];
                                                                var productName = product.Keys.First();
                                                                product[productName]["Time"] = timeGroups[j];
                                                            }


                                                            // Eliminar productos sin tiempo de la copia
                                                            productsCopy = productsCopy.Where(product => product.Values.First()["Time"] != "§§§§").ToList();

                                                            //primera busqueda sin producto
                                                            var coincidenciasOperaciones = OperationsDictionary
                                                                  .Where(pair => pair.Key.Item1 == PathResume.PlantId && pair.Key.Item2 == PathResume.AreaId && pair.Key.Item3 == PathResume.DistributionId)
                                                                  .Select(pair => new
                                                                  {
                                                                      Operation = pair.Value,
                                                                      Similarity = (pair.Value.Code == ExcelOpCode && pair.Value.Description == ExcelOpDescription ? 1 : 0)
                                                                  })
                                                                  .OrderByDescending(result => result.Similarity)
                                                                  .FirstOrDefault();

                                                            //busqueda con producto
                                                            if (productsCopy.Count > 0 && coincidenciasOperaciones == null)
                                                            {
                                                                //Coincidencia de producto
                                                                string productCode = productsCopy[0].Keys.First();

                                                                if (ExcelOpCode == "CC" || ExcelOpCode == "cc")
                                                                {
                                                                    ExcelOpCode = $"{productCode} - {ExcelOpCode}";
                                                                }

                                                                if (ExcelOpCode.DiceCoefficient("FALTA GOS") > 0.8)
                                                                {
                                                                    ExcelOpCode = $"{productCode} - {ExcelOpCode}";
                                                                }

                                                                coincidenciasOperaciones = OperationsDictionary
                                                                  .Where(pair => pair.Key.Item1 == PathResume.PlantId && pair.Key.Item2 == PathResume.AreaId && pair.Key.Item3 == PathResume.DistributionId)
                                                                  .Select(pair => new
                                                                  {
                                                                      Operation = pair.Value,
                                                                      Similarity = (pair.Value.Code == ExcelOpCode && pair.Value.Description == ExcelOpDescription ? 1 : 0)
                                                                  })
                                                                  .OrderByDescending(result => result.Similarity)
                                                                  .FirstOrDefault();

                                                                if (coincidenciasOperaciones != null && coincidenciasOperaciones.Similarity > 0.7)
                                                                {
                                                                    if (coincidenciasOperaciones.Operation.DistributionId == coincidenciasDistributions.Distribution.DistributionId)
                                                                    {
                                                                        PathResume.OperationId = coincidenciasOperaciones.Operation.OperationId;
                                                                    }
                                                                }
                                                            }
                                                            else if (coincidenciasOperaciones != null && coincidenciasOperaciones.Similarity > 0.7)
                                                            {
                                                                if (coincidenciasOperaciones.Operation.DistributionId == coincidenciasDistributions.Distribution.DistributionId)
                                                                {
                                                                    PathResume.OperationId = coincidenciasOperaciones.Operation.OperationId;
                                                                }
                                                            }

                                                            if (PathResume.OperationId > 0)
                                                            {
                                                                Debug.WriteLine($"La Operacion {ExcelOpCode} - {ExcelOpDescription} Existe :) !!! ");
                                                                //Aqui una verificacion de informacion, si algun dato en los tiempos cambia, hay que actualizar el json//
                                                                // Update a la base de datos

                                                                if (productsCopy.Count > 0)
                                                                {
                                                                    //Coincidencia de producto
                                                                    string productCode = productsCopy[0].Keys.First();

                                                                    var coincidenciasProduct = Products.Select(pair => new
                                                                    {
                                                                        Product = pair,
                                                                        Similarity = 1 - pair.Code.JaccardDistance(productCode)
                                                                    }).OrderByDescending(result => result.Similarity).FirstOrDefault();

                                                                    // Ajusta este umbral según la necesidad
                                                                    if (coincidenciasProduct != null && coincidenciasProduct.Similarity > 0.5)
                                                                    {
                                                                        PathResume.ProductID = coincidenciasProduct.Product.ProductId;
                                                                    }

                                                                    var finalproduct = coincidenciasProduct.Product;
                                                                    var finalDistribution = await _context.Distributions.Where(p => p.DistributionId == PathResume.DistributionId).FirstOrDefaultAsync();


                                                                    if (finalproduct != null)
                                                                    {
                                                                        if (finalproduct.Distributions != null)
                                                                        {
                                                                            if (!finalproduct.Distributions.Any(d => d.DistributionId == finalDistribution.DistributionId))
                                                                            {
                                                                                finalproduct.Distributions.Add(finalDistribution);
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            finalproduct.Distributions = new List<Distribution>();
                                                                            finalproduct.Distributions.Add(finalDistribution);
                                                                        }
                                                                    }

                                                                    await dbContext.SaveChangesAsync();


                                                                    //aqui va la creacion de rutas
                                                                    TreeItemData? mejorCoincidenciaHOE = null;
                                                                    TreeItemData? mejorCoincidenciaGOS = null;
                                                                    TreeItemData? mejorCoincidenciaCCP = null;

                                                                    mejorCoincidenciaHOE = _treeService.EncontrarNodoMejorCoincidencia(rootNodeHOE, planta, "produccion", coincidenciasAreas.Area, coincidenciasDistributions.Distribution, coincidenciasProduct.Product);

                                                                    if (mejorCoincidenciaHOE != null)
                                                                    {
                                                                        PathResume.HOE = mejorCoincidenciaHOE.Ruta;
                                                                        Debug.WriteLine("HOE: " + mejorCoincidenciaHOE.Ruta);
                                                                        Console.WriteLine("HOE: " + mejorCoincidenciaHOE.Ruta);
                                                                    }

                                                                    mejorCoincidenciaGOS = _treeService.EncontrarNodoMejorCoincidencia(rootNodeGOS, planta, "produccion", coincidenciasAreas.Area, coincidenciasDistributions.Distribution, coincidenciasProduct.Product);

                                                                    if (mejorCoincidenciaGOS != null)
                                                                    {
                                                                        PathResume.GOS = mejorCoincidenciaGOS.Ruta;
                                                                        Debug.WriteLine("GOS: " + mejorCoincidenciaGOS.Ruta);
                                                                        Console.WriteLine("GOS: " + mejorCoincidenciaGOS.Ruta);
                                                                    }

                                                                    mejorCoincidenciaCCP = _treeService.EncontrarNodoMejorCoincidencia(rootNodeCCP, planta, "produccion", coincidenciasAreas.Area, coincidenciasDistributions.Distribution, coincidenciasProduct.Product);

                                                                    if (mejorCoincidenciaCCP != null)
                                                                    {
                                                                        PathResume.CCP = mejorCoincidenciaCCP.Ruta;
                                                                        Debug.WriteLine("CCP: " + mejorCoincidenciaCCP.Ruta);
                                                                        Console.WriteLine("CCP: " + mejorCoincidenciaCCP.Ruta);
                                                                    }

                                                                    //opetenemos la ruta si existge

                                                                    SOSCodePath? ExistCodePath = await dbContext.CodePaths.Where(p => p.AssyChardId == AssyChartExist.AssyChardId && p.Code == coincidenciasOperaciones.Operation.Code).FirstOrDefaultAsync();

                                                                    if (ExistCodePath is null)
                                                                    {
                                                                        // no existe se crea
                                                                        //procedimiento de path
                                                                        SOSCodePath CodePath = new SOSCodePath();

                                                                        CodePath.Code = coincidenciasOperaciones.Operation.Code;


                                                                        if (mejorCoincidenciaHOE != null)
                                                                        {
                                                                            CodePath.HOE = mejorCoincidenciaHOE.Ruta;
                                                                        }

                                                                        if (mejorCoincidenciaGOS != null)
                                                                        {
                                                                            CodePath.GOS = mejorCoincidenciaGOS.Ruta;
                                                                        }

                                                                        if (mejorCoincidenciaCCP != null)
                                                                        {
                                                                            CodePath.CCP = mejorCoincidenciaCCP.Ruta;
                                                                        }


                                                                        //Añadimso distribucion y Producto

                                                                        CodePath.DistributionId = (int)PathResume.DistributionId;
                                                                        CodePath.ProductId = PathResume.ProductID;

                                                                        CodePath.AssyChardId = AssyChartExist.AssyChardId;

                                                                        //Crear Code Path Version de using dbContext
                                                                        dbContext.CodePaths.Add(CodePath);
                                                                        //aqui se añade el path creado
                                                                        if (AssyChartExist.RoutesProductsAssyChart != null)
                                                                        {
                                                                            AssyChartExist.RoutesProductsAssyChart.Add(CodePath);
                                                                        }
                                                                        else
                                                                        {
                                                                            AssyChartExist.RoutesProductsAssyChart = new List<SOSCodePath>();
                                                                            AssyChartExist.RoutesProductsAssyChart.Add(CodePath);
                                                                        }
                                                                        dbContext.SaveChanges();
                                                                    }
                                                                    else
                                                                    {
                                                                        //asignacion directa
                                                                        if (mejorCoincidenciaHOE != null)
                                                                        {
                                                                            ExistCodePath.HOE = mejorCoincidenciaHOE.Ruta;
                                                                        }

                                                                        if (mejorCoincidenciaGOS != null)
                                                                        {
                                                                            ExistCodePath.GOS = mejorCoincidenciaGOS.Ruta;
                                                                        }

                                                                        if (mejorCoincidenciaCCP != null)
                                                                        {
                                                                            ExistCodePath.CCP = mejorCoincidenciaCCP.Ruta;
                                                                        }
                                                                        if (ExistCodePath.DistributionId != coincidenciasDistributions.Distribution.DistributionId)
                                                                        {
                                                                            ExistCodePath.DistributionId = (int)PathResume.DistributionId;
                                                                        }
                                                                        if (ExistCodePath.ProductId != coincidenciasProduct.Product.ProductId)
                                                                        {
                                                                            ExistCodePath.ProductId = (int)PathResume.ProductID;

                                                                        }

                                                                        //la ruta ya existe se actualiza

                                                                        //dbContext.CodePaths.Update(ExistCodePath);

                                                                    }



                                                                    bool isUpdate = false;
                                                                    OperationForUpdateDto OperationforUpdate = _mapper.Map<OperationForUpdateDto>(coincidenciasOperaciones.Operation);

                                                                    var productToUpdate = productsCopy.FirstOrDefault(product => product.Keys.First() == productCode);

                                                                    if (productToUpdate != null && productCode != OperationforUpdate.ProductName)
                                                                    {
                                                                        OperationforUpdate.ProductName = productCode;
                                                                        isUpdate = true;
                                                                    }

                                                                    if (productToUpdate != null && productToUpdate.Values.First()["NameTime"] != OperationforUpdate.NameTime)
                                                                    {
                                                                        OperationforUpdate.NameTime = productToUpdate.Values.First()["NameTime"];
                                                                        isUpdate = true;
                                                                    }

                                                                    if (productToUpdate != null && productToUpdate.Values.First()["Time"] != OperationforUpdate.Time)
                                                                    {
                                                                        OperationforUpdate.Time = productToUpdate.Values.First()["Time"];
                                                                        isUpdate = true;
                                                                    }

                                                                    if (productToUpdate != null && productToUpdate.Values.First()["AdditionalTime"] != OperationforUpdate.AdditionalTime)
                                                                    {
                                                                        OperationforUpdate.AdditionalTime = productToUpdate.Values.First()["AdditionalTime"];
                                                                        isUpdate = true;
                                                                    }

                                                                    if (productToUpdate != null && productToUpdate.Values.First()["StandardTime"] != OperationforUpdate.StandardTime)
                                                                    {
                                                                        OperationforUpdate.StandardTime = productToUpdate.Values.First()["StandardTime"];
                                                                        isUpdate = true;
                                                                    }


                                                                    if (OperationforUpdate.restrictionorcomment != ExcelCommentaryOrRestriction)
                                                                    {
                                                                        OperationforUpdate.restrictionorcomment = ExcelCommentaryOrRestriction;
                                                                        isUpdate = true;

                                                                    }

                                                                    if (isUpdate)
                                                                    {
                                                                        var operationEntity = await dbContext.Operations.Where(o => o.DistributionId == (int)PathResume.DistributionId && o.OperationId == (int)PathResume.OperationId).FirstOrDefaultAsync();
                                                                        if (operationEntity == null)
                                                                        {
                                                                            DocumentError = true;
                                                                            eMailBody += $"\\n No es posible actualizar la operacion: {coincidenciasOperaciones.Operation.Code} Distribucion: {coincidenciasDistributions.Distribution.Description}" +
                                                                                $" Pagina: {p} - {pageName}";
                                                                        }
                                                                        _mapper.Map(OperationforUpdate, operationEntity);
                                                                        dbContext.SaveChanges();
                                                                    }


                                                                }
                                                                else
                                                                {
                                                                    DocumentError = true;
                                                                    eMailBody += $"\\n Faltan datos en el documento..." +
                                                                        $" Rango de celdas F{row.RowNumber()}-Y{row.RowNumber()}" +
                                                                        $" Pagina: {p} - {pageName}" +
                                                                        $" Distribucion: {coincidenciasDistributions.Distribution.Description} Operacion: {coincidenciasOperaciones.Operation.Code}";

                                                                }
                                                            }
                                                            else
                                                            {//La operacion no existe
                                                                Debug.WriteLine($"La Operacion  NO EXISTE {ExcelOpCode} - {ExcelOpDescription} NO EXISTE :c  ");

                                                                //creacion de json del producto con tiempos

                                                                if (productsCopy.Count > 0)
                                                                {
                                                                    //Coincidencia de producto
                                                                    string productCode = productsCopy[0].Keys.First();

                                                                    var coincidenciasProduct = Products.Select(pair => new
                                                                    {
                                                                        Product = pair,
                                                                        Similarity = 1 - pair.Code.JaccardDistance(productCode)
                                                                    }).OrderByDescending(result => result.Similarity).FirstOrDefault();

                                                                    // Ajusta este umbral según la necesidad
                                                                    if (coincidenciasProduct != null && coincidenciasProduct.Similarity > 0.5)
                                                                    {
                                                                        PathResume.ProductID = coincidenciasProduct.Product.ProductId;
                                                                    }

                                                                    var finalproduct = coincidenciasProduct.Product;
                                                                    var finalDistribution = await _context.Distributions.Where(p => p.DistributionId == PathResume.DistributionId).FirstOrDefaultAsync();


                                                                    if (finalproduct != null)
                                                                    {
                                                                        if (finalproduct.Distributions != null)
                                                                        {
                                                                            if (!finalproduct.Distributions.Any(d => d.DistributionId == finalDistribution.DistributionId))
                                                                            {
                                                                                finalproduct.Distributions.Add(finalDistribution);
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            finalproduct.Distributions = new List<Distribution>();
                                                                            finalproduct.Distributions.Add(finalDistribution);
                                                                        }
                                                                    }

                                                                    await dbContext.SaveChangesAsync();

                                                                    //aqui va la creacion de rutas
                                                                    TreeItemData? mejorCoincidenciaHOE = null;
                                                                    TreeItemData? mejorCoincidenciaGOS = null;
                                                                    TreeItemData? mejorCoincidenciaCCP = null;

                                                                    mejorCoincidenciaHOE = _treeService.EncontrarNodoMejorCoincidencia(rootNodeHOE, planta, "produccion", coincidenciasAreas.Area, coincidenciasDistributions.Distribution, coincidenciasProduct.Product);

                                                                    if (mejorCoincidenciaHOE != null)
                                                                    {
                                                                        PathResume.HOE = mejorCoincidenciaHOE.Ruta;
                                                                        Debug.WriteLine("HOE: " + mejorCoincidenciaHOE.Ruta);
                                                                        Console.WriteLine("HOE: " + mejorCoincidenciaHOE.Ruta);
                                                                    }

                                                                    mejorCoincidenciaGOS = _treeService.EncontrarNodoMejorCoincidencia(rootNodeGOS, planta, "produccion", coincidenciasAreas.Area, coincidenciasDistributions.Distribution, coincidenciasProduct.Product);

                                                                    if (mejorCoincidenciaGOS != null)
                                                                    {
                                                                        PathResume.GOS = mejorCoincidenciaGOS.Ruta;
                                                                        Debug.WriteLine("GOS: " + mejorCoincidenciaGOS.Ruta);
                                                                        Console.WriteLine("GOS: " + mejorCoincidenciaGOS.Ruta);
                                                                    }

                                                                    mejorCoincidenciaCCP = _treeService.EncontrarNodoMejorCoincidencia(rootNodeCCP, planta, "produccion", coincidenciasAreas.Area, coincidenciasDistributions.Distribution, coincidenciasProduct.Product);

                                                                    if (mejorCoincidenciaCCP != null)
                                                                    {
                                                                        PathResume.CCP = mejorCoincidenciaCCP.Ruta;
                                                                        Debug.WriteLine("CCP: " + mejorCoincidenciaCCP.Ruta);
                                                                        Console.WriteLine("CCP: " + mejorCoincidenciaCCP.Ruta);
                                                                    }

                                                                    SOSCodePath? ExistCodePath = await _context.CodePaths.Where(p => p.AssyChardId == AssyChartExist.AssyChardId && p.Code == ExcelOpCode).FirstOrDefaultAsync();

                                                                    if (ExistCodePath is null)
                                                                    {
                                                                        // no existe se crea
                                                                        //procedimiento de path
                                                                        SOSCodePath CodePath = new SOSCodePath();

                                                                        CodePath.Code = ExcelOpCode;


                                                                        if (mejorCoincidenciaHOE != null)
                                                                        {
                                                                            CodePath.HOE = mejorCoincidenciaHOE.Ruta;
                                                                        }

                                                                        if (mejorCoincidenciaGOS != null)
                                                                        {
                                                                            CodePath.GOS = mejorCoincidenciaGOS.Ruta;
                                                                        }

                                                                        if (mejorCoincidenciaCCP != null)
                                                                        {
                                                                            CodePath.CCP = mejorCoincidenciaCCP.Ruta;
                                                                        }


                                                                        //Añadimso distribucion y Producto

                                                                        CodePath.DistributionId = (int)PathResume.DistributionId;
                                                                        CodePath.ProductId = PathResume.ProductID;

                                                                        CodePath.AssyChardId = AssyChartExist.AssyChardId;


                                                                        //Crear Code Path Version de using dbContext
                                                                        dbContext.CodePaths.Add(CodePath);
                                                                        //aqui se añade el path creado
                                                                        if (AssyChartExist.RoutesProductsAssyChart != null)
                                                                        {
                                                                            AssyChartExist.RoutesProductsAssyChart.Add(CodePath);
                                                                        }
                                                                        else
                                                                        {
                                                                            AssyChartExist.RoutesProductsAssyChart = new List<SOSCodePath>();
                                                                            AssyChartExist.RoutesProductsAssyChart.Add(CodePath);
                                                                        }
                                                                        dbContext.SaveChanges();
                                                                    }
                                                                    else
                                                                    {
                                                                        //asignacion directa
                                                                        if (mejorCoincidenciaHOE != null)
                                                                        {
                                                                            ExistCodePath.HOE = mejorCoincidenciaHOE.Ruta;
                                                                        }

                                                                        if (mejorCoincidenciaGOS != null)
                                                                        {
                                                                            ExistCodePath.GOS = mejorCoincidenciaGOS.Ruta;
                                                                        }

                                                                        if (mejorCoincidenciaCCP != null)
                                                                        {
                                                                            ExistCodePath.CCP = mejorCoincidenciaCCP.Ruta;
                                                                        }
                                                                        if (ExistCodePath.DistributionId != coincidenciasDistributions.Distribution.DistributionId)
                                                                        {
                                                                            ExistCodePath.DistributionId = (int)PathResume.DistributionId;
                                                                        }
                                                                        if (ExistCodePath.ProductId != coincidenciasProduct.Product.ProductId)
                                                                        {
                                                                            ExistCodePath.ProductId = (int)PathResume.ProductID;
                                                                        }

                                                                        //la ruta ya existe se actualiza

                                                                        //dbContext.CodePaths.Update(ExistCodePath);



                                                                    }

                                                                    var ProductJson = productsCopy.FirstOrDefault(product => product.Keys.First() == productCode);

                                                                    var operationForCreate = _mapper.Map<OperationForCreationDto>(new OperationForCreationDto() { Code = ExcelOpCode, Description = ExcelOpDescription, IsActive = true });


                                                                    operationForCreate.restrictionorcomment = ExcelCommentaryOrRestriction;

                                                                    operationForCreate.ProductName = productCode;
                                                                    operationForCreate.NameTime = ProductJson.Values.First()["NameTime"];
                                                                    operationForCreate.Time = ProductJson.Values.First()["Time"];
                                                                    operationForCreate.AdditionalTime = ProductJson.Values.First()["AdditionalTime"];
                                                                    operationForCreate.StandardTime = ProductJson.Values.First()["StandardTime"];


                                                                    var finalOperation = _mapper.Map<Operation>(operationForCreate);



                                                                    var distribution = await dbContext.Distributions.Where(o => o.AreaId == (int)PathResume.AreaId && o.DistributionId == (int)PathResume.DistributionId).FirstOrDefaultAsync();
                                                                    if (distribution != null)
                                                                    {
                                                                        distribution.Operations.Add(finalOperation);
                                                                    }
                                                                    dbContext.SaveChanges();

                                                                    OperationsDictionary.Add(((int)PathResume.PlantId, (int)PathResume.AreaId, (int)PathResume.DistributionId, finalOperation.OperationId), finalOperation);
                                                                    CountCreateOperation++;
                                                                }
                                                                else
                                                                {
                                                                    //Debug.WriteLine($"La Operacion  NO EXISTE {ExcelOpCode} - {ExcelOpDescription} NO EXISTE :c  ");
                                                                    DocumentError = true;
                                                                    eMailBody += $"\\n Faltan datos en el documento..." +
                                                                      $" Rango de celdas F{row.RowNumber()}-Y{row.RowNumber()}" +
                                                                      $" Pagina: {p} - {pageName}" +
                                                                      $" Distribucion: {coincidenciasDistributions.Distribution.Description} Operacion: {coincidenciasOperaciones.Operation.Code}";
                                                                }

                                                            }

                                                        }
                                                        else if (cellB.IsMerged() && row.RowNumber() >= rowStartOperation)
                                                        {
                                                            //Finalizamos recorrido de renglones, ya no hay mas operaciones
                                                            break;
                                                        }
                                                    }
                                                    //if (DocumentError) {
                                                    //    //provicional, busco finalizar rapido la ejecucion al encontrar un error
                                                    //    break;
                                                    //}
                                                }//end if distribution >0
                                                else
                                                {
                                                    ////distribution no existe
                                                    string codeGen = ExcelDistDescription;

                                                    SlugHelper slugHelper = new SlugHelper();
                                                    string slug = slugHelper.GenerateSlug(codeGen);

                                                    var distributionForCreate = _mapper.Map<DistributionForCreationDto>(new DistributionForCreationDto() { Code = slug, Description = ExcelDistDescription, IsActive = true });
                                                    var finalDistribution = _mapper.Map<Distribution>(distributionForCreate);


                                                    var area = await dbContext.Areas.Where(a => a.PlantId == (int)PathResume.PlantId && a.AreaId == (int)PathResume.AreaId).FirstOrDefaultAsync();

                                                    if (area != null)
                                                    {
                                                        area.Distributions.Add(finalDistribution);
                                                    }

                                                    await dbContext.SaveChangesAsync();

                                                    PathResume.DistributionId = finalDistribution.DistributionId;
                                                    DistributionsDictionary.Add(((int)PathResume.PlantId, (int)PathResume.AreaId, (int)PathResume.DistributionId), finalDistribution);


                                                    coincidenciasDistributions = DistributionsDictionary
                                                        .Where(pair => pair.Key.Item1 == PathResume.PlantId && pair.Key.Item2 == PathResume.AreaId && pair.Key.Item3 == PathResume.DistributionId)
                                                        .Select(pair => new
                                                        {
                                                            Distribution = pair.Value,
                                                            // Se combina la similitud a nivel de caracteres y palabras
                                                            Similarity = 1.0
                                                        })
                                                        .OrderByDescending(result => result.Similarity)
                                                    .FirstOrDefault();

                                                    if (coincidenciasDistributions != null && coincidenciasDistributions.Similarity > 0.95)
                                                    {
                                                        PathResume.DistributionId = coincidenciasDistributions.Distribution.DistributionId;
                                                        PathResume.DescripcionDistribucion = coincidenciasDistributions.Distribution.Description;
                                                    }

                                                    //Si la distribucion no existe, el assy chart tampoco existe
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

                                                        //aqui va la transiction 
                                                        var finalasssychart = _mapper.Map<AssyChart>(assychartForCreate);
                                                        dbContext.AssyCharts.Add(finalasssychart);
                                                        dbContext.SaveChanges();
                                                        Debug.WriteLine($"dbContext SaveChanges Succesfull ");

                                                        CountCreateAssycchart++;

                                                        Debug.WriteLine($"Create assychart id {finalasssychart.AssyChardId} plantid {(int)PathResume.PlantId} areaid {(int)PathResume.AreaId} distributionid {(int)PathResume.DistributionId} ");
                                                        AssyChartExist = finalasssychart;
                                                    }

                                                    string lastOperationName = null; // Para almacenar el último nombre de operación
                                                    //Obtencion de los tiempos por renglon en base a operacion
                                                    foreach (var row in rows.SkipWhile(r => r.RowNumber() < startingRow.RowNumber()))
                                                    {
                                                        PathResume.OperationId = null;
                                                        // Obtener la celda en la columna B para cada renglón
                                                        var cellB = row.Cell("B");

                                                        // Verificar si la celda no está combinada y es mayor o igual a la fila 12
                                                        if (!cellB.IsMerged() && row.RowNumber() >= rowStartOperation)
                                                        {
                                                            var CellOpCode = row.Cell("C");
                                                            var CellOpDesc = row.Cell("D");
                                                            var CellCommentaryOrRestriction = row.Cell("E");

                                                            var ExcelOpCode = CellOpCode.Value.ToString() != "" ? CellOpCode.Value.ToString() : "";
                                                            var ExcelOpDescription = CellOpDesc.Value.ToString() != "" ? CellOpDesc.Value.ToString() : "";


                                                            if (!string.IsNullOrEmpty(ExcelOpDescription))
                                                            {
                                                                lastOperationName = ExcelOpDescription;
                                                            }

                                                            // Si no hay un nombre en la columna B, reutilizamos el último nombre de operación
                                                            if (lastOperationName != null)
                                                            {
                                                                ExcelOpDescription = lastOperationName;
                                                            }

                                                            var ExcelCommentaryOrRestriction = CellCommentaryOrRestriction.Value.ToString() != "" ? CellCommentaryOrRestriction.Value.ToString() : "";

                                                            if (string.IsNullOrEmpty(ExcelOpCode) && string.IsNullOrEmpty(ExcelOpDescription))
                                                            {
                                                                //si es renglon vacio brincamos al siguiente
                                                                continue;
                                                            }
                                                            else if (string.IsNullOrEmpty(ExcelOpCode) && !string.IsNullOrEmpty(ExcelOpDescription))
                                                            {
                                                                DocumentError = true;
                                                                eMailBody += $"\\n Falta No. Operacion..." +
                                                                    $" Rango de celdas C{row.RowNumber()}" +
                                                                    $" Pagina: {p} - {pageName}" +
                                                                    $" Distribucion: {coincidenciasDistributions.Distribution.Description}";
                                                            }
                                                            else if (!string.IsNullOrEmpty(ExcelOpCode) && string.IsNullOrEmpty(ExcelOpDescription))
                                                            {
                                                                DocumentError = true;
                                                                eMailBody += $"\\n Falta Nombre de operacion..." +
                                                                    $" Rango de celdas C{row.RowNumber()}" +
                                                                    $" Pagina: {p} - {pageName}" +
                                                                    $" Distribucion: {coincidenciasDistributions.Distribution.Description}";
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
                                                            //una copia de los productos vacios para añádir los tiempso correspodnientes
                                                            List<Dictionary<string, Dictionary<string, string>>> productsCopy = ObjectCloner.ObjectCloner.DeepClone(products);
                                                            for (int j = 0; j < productsCopy.Count; j++)
                                                            {
                                                                var product = productsCopy[j];
                                                                var productName = product.Keys.First();
                                                                product[productName]["Time"] = timeGroups[j];
                                                            }


                                                            // Eliminar productos sin tiempo de la copia
                                                            productsCopy = productsCopy.Where(product => product.Values.First()["Time"] != "§§§§").ToList();

                                                            //La operacion No existe, si la distribucion no existe hay que crearla
                                                            Debug.WriteLine($"La Operacion NO EXISTE {ExcelOpCode} - {ExcelOpDescription} NO Existe :c !!! ");
                                                            //creacion de json del producto con tiempos


                                                            if (productsCopy.Count > 0)
                                                            {
                                                                //Coincidencia de producto
                                                                string productCode = productsCopy[0].Keys.First();
                                                                //si es operacion de calidad añadimos el producto antes de cc
                                                                if (ExcelOpCode == "CC" || ExcelOpCode == "cc")
                                                                {
                                                                    ExcelOpCode = $"{productCode} - {ExcelOpCode}";
                                                                }


                                                                if (ExcelOpCode.DiceCoefficient("FALTA GOS") > 0.8)
                                                                {
                                                                    ExcelOpCode = $"{productCode} - {ExcelOpCode}";
                                                                }

                                                                var coincidenciasProduct = Products.Select(pair => new
                                                                {
                                                                    Product = pair,
                                                                    Similarity = 1 - pair.Code.JaccardDistance(productCode)
                                                                }).OrderByDescending(result => result.Similarity).FirstOrDefault();

                                                                // Ajusta este umbral según la necesidad
                                                                if (coincidenciasProduct != null && coincidenciasProduct.Similarity > 0.5)
                                                                {
                                                                    PathResume.ProductID = coincidenciasProduct.Product.ProductId;
                                                                }

                                                                var finalproduct = coincidenciasProduct.Product;
                                                                Debug.WriteLine("GET product dbContext");

                                                                if (finalproduct != null)
                                                                {
                                                                    if (finalproduct.Distributions != null)
                                                                    {
                                                                        if (!finalproduct.Distributions.Any(d => d.DistributionId == finalDistribution.DistributionId))
                                                                            finalproduct.Distributions.Add(finalDistribution);
                                                                    }
                                                                    else
                                                                    {
                                                                        finalproduct.Distributions = new List<Distribution>();
                                                                        finalproduct.Distributions.Add(finalDistribution);

                                                                    }
                                                                }
                                                                await dbContext.SaveChangesAsync();

                                                                var ProductJson = productsCopy.FirstOrDefault(product => product.Keys.First() == productCode);

                                                                var operationForCreate = _mapper.Map<OperationForCreationDto>(new OperationForCreationDto() { Code = ExcelOpCode, Description = ExcelOpDescription, IsActive = true });
                                                                operationForCreate.restrictionorcomment = ExcelCommentaryOrRestriction;


                                                                operationForCreate.ProductName = productCode;
                                                                operationForCreate.NameTime = ProductJson.Values.First()["NameTime"];
                                                                operationForCreate.Time = ProductJson.Values.First()["Time"];
                                                                operationForCreate.AdditionalTime = ProductJson.Values.First()["AdditionalTime"];
                                                                operationForCreate.StandardTime = ProductJson.Values.First()["StandardTime"];


                                                                var finalOperation = _mapper.Map<Operation>(operationForCreate);

                                                                //aqui va la creacion de rutas
                                                                TreeItemData? mejorCoincidenciaHOE = null;
                                                                TreeItemData? mejorCoincidenciaGOS = null;
                                                                TreeItemData? mejorCoincidenciaCCP = null;

                                                                mejorCoincidenciaHOE = _treeService.EncontrarNodoMejorCoincidencia(rootNodeHOE, planta, "produccion", coincidenciasAreas.Area, coincidenciasDistributions.Distribution, coincidenciasProduct.Product);

                                                                if (mejorCoincidenciaHOE != null)
                                                                {
                                                                    PathResume.HOE = mejorCoincidenciaHOE.Ruta;
                                                                    Debug.WriteLine("HOE: " + mejorCoincidenciaHOE.Ruta);
                                                                    Console.WriteLine("HOE: " + mejorCoincidenciaHOE.Ruta);
                                                                }

                                                                mejorCoincidenciaGOS = _treeService.EncontrarNodoMejorCoincidencia(rootNodeGOS, planta, "produccion", coincidenciasAreas.Area, coincidenciasDistributions.Distribution, coincidenciasProduct.Product);

                                                                if (mejorCoincidenciaGOS != null)
                                                                {
                                                                    PathResume.GOS = mejorCoincidenciaGOS.Ruta;
                                                                    Debug.WriteLine("GOS: " + mejorCoincidenciaGOS.Ruta);
                                                                    Console.WriteLine("GOS: " + mejorCoincidenciaGOS.Ruta);
                                                                }

                                                                mejorCoincidenciaCCP = _treeService.EncontrarNodoMejorCoincidencia(rootNodeCCP, planta, "produccion", coincidenciasAreas.Area, coincidenciasDistributions.Distribution, coincidenciasProduct.Product);

                                                                if (mejorCoincidenciaCCP != null)
                                                                {
                                                                    PathResume.CCP = mejorCoincidenciaCCP.Ruta;
                                                                    Debug.WriteLine("CCP: " + mejorCoincidenciaCCP.Ruta);
                                                                    Console.WriteLine("CCP: " + mejorCoincidenciaCCP.Ruta);
                                                                }

                                                                //assychart es nullo en este punto
                                                                SOSCodePath? ExistCodePath = await _context.CodePaths.Where(p => p.AssyChardId == AssyChartExist.AssyChardId && p.Code == finalOperation.Code).FirstOrDefaultAsync();

                                                                if (ExistCodePath is null)
                                                                {
                                                                    // no existe se crea
                                                                    //procedimiento de path
                                                                    SOSCodePath CodePath = new SOSCodePath();

                                                                    CodePath.Code = finalOperation.Code;


                                                                    if (mejorCoincidenciaHOE != null)
                                                                    {
                                                                        CodePath.HOE = mejorCoincidenciaHOE.Ruta;
                                                                    }

                                                                    if (mejorCoincidenciaGOS != null)
                                                                    {
                                                                        CodePath.GOS = mejorCoincidenciaGOS.Ruta;
                                                                    }

                                                                    if (mejorCoincidenciaCCP != null)
                                                                    {
                                                                        CodePath.CCP = mejorCoincidenciaCCP.Ruta;
                                                                    }

                                                                    //Añadimso distribucion y Producto

                                                                    CodePath.DistributionId = (int)PathResume.DistributionId;
                                                                    CodePath.ProductId = PathResume.ProductID;

                                                                    CodePath.AssyChardId = AssyChartExist.AssyChardId;

                                                                    //Crear Code Path Version de using dbContext
                                                                    dbContext.CodePaths.Add(CodePath);
                                                                    //aqui se añade el path creado
                                                                    if (AssyChartExist.RoutesProductsAssyChart != null)
                                                                    {
                                                                        AssyChartExist.RoutesProductsAssyChart.Add(CodePath);
                                                                    }
                                                                    else
                                                                    {
                                                                        AssyChartExist.RoutesProductsAssyChart = new List<SOSCodePath>();
                                                                        AssyChartExist.RoutesProductsAssyChart.Add(CodePath);
                                                                    }
                                                                    dbContext.SaveChanges();
                                                                }
                                                                else
                                                                {
                                                                    //asignacion directa
                                                                    if (mejorCoincidenciaHOE != null)
                                                                    {
                                                                        ExistCodePath.HOE = mejorCoincidenciaHOE.Ruta;
                                                                    }

                                                                    if (mejorCoincidenciaGOS != null)
                                                                    {
                                                                        ExistCodePath.GOS = mejorCoincidenciaGOS.Ruta;
                                                                    }

                                                                    if (mejorCoincidenciaCCP != null)
                                                                    {
                                                                        ExistCodePath.CCP = mejorCoincidenciaCCP.Ruta;
                                                                    }
                                                                    if (ExistCodePath.DistributionId != coincidenciasDistributions.Distribution.DistributionId)
                                                                    {
                                                                        ExistCodePath.DistributionId = (int)PathResume.DistributionId;
                                                                    }
                                                                    if (ExistCodePath.ProductId != coincidenciasProduct.Product.ProductId)
                                                                    {
                                                                        ExistCodePath.ProductId = (int)PathResume.ProductID;
                                                                    }

                                                                    //la ruta ya existe se actualiza

                                                                    //dbContext.CodePaths.Update(ExistCodePath);

                                                                }


                                                                var distribution = await _context.Distributions.Where(o => o.AreaId == (int)PathResume.AreaId && o.DistributionId == (int)PathResume.DistributionId).FirstOrDefaultAsync();
                                                                if (distribution != null)
                                                                {
                                                                    distribution.Operations.Add(finalOperation);
                                                                }
                                                                dbContext.SaveChanges();

                                                                OperationsDictionary.Add(((int)PathResume.PlantId, (int)PathResume.AreaId, (int)PathResume.DistributionId, finalOperation.OperationId), finalOperation);
                                                                CountCreateOperation++;
                                                            }
                                                            else
                                                            {
                                                                //Debug.WriteLine($"Distribucio no existe y no hay productos ");
                                                                DocumentError = true;
                                                                eMailBody += $"\\n Faltan datos en el documento..." +
                                                                  $" Rango de celdas F{row.RowNumber()}-Y{row.RowNumber()}" +
                                                                  $" Pagina: {p} - {pageName}" +
                                                                  $" Distribucion: {coincidenciasDistributions.Distribution.Description}";
                                                            }


                                                        }

                                                    }

                                                    //if (DocumentError)
                                                    //{
                                                    //    break;
                                                    //}
                                                }//end else distribuccion no existe

                                            }//end if area si existe
                                            else //area no existe
                                            {
                                                ////El area no existe, por lo que la distribuccion tampoco existe 
                                                SlugHelper slugHelper = new SlugHelper();

                                                var areaForCreate = _mapper.Map<AreaForCreationDto>(new AreaForCreationDto() { Code = ExcelAreaCode, Description = ExcelAreaCode, IsActive = true });

                                                var finalArea = _mapper.Map<Area>(areaForCreate);
                                                finalArea.PlantId = (int)PathResume.PlantId;

                                                await _supervisorMobilityRepository.AddArea(finalArea);
                                                await _supervisorMobilityRepository.AddAreaForPlantAsync((int)PathResume.PlantId, finalArea);
                                                await _supervisorMobilityRepository.SaveChangesAsync();
                                                PathResume.AreaId = finalArea.AreaId;

                                                AreasDictionary.Add(((int)PathResume.PlantId, (int)PathResume.AreaId), finalArea);

                                                //Distribucion desde aqui
                                                string codeGen = ExcelDistDescription;
                                                string slug = slugHelper.GenerateSlug(codeGen);

                                                var distributionForCreate = _mapper.Map<DistributionForCreationDto>(new DistributionForCreationDto() { Code = slug, Description = ExcelDistDescription, IsActive = true });
                                                var finalDistribution = _mapper.Map<Distribution>(distributionForCreate);


                                                var area = await dbContext.Areas.Where(a => a.PlantId == (int)PathResume.PlantId && a.AreaId == (int)PathResume.AreaId).FirstOrDefaultAsync();

                                                if (area != null)
                                                {
                                                    area.Distributions.Add(finalDistribution);
                                                }

                                                await dbContext.SaveChangesAsync();

                                                PathResume.DistributionId = finalDistribution.DistributionId;
                                                DistributionsDictionary.Add(((int)PathResume.PlantId, (int)PathResume.AreaId, (int)PathResume.DistributionId), finalDistribution);



                                                //Si la distribucion no existe, el assy chart tampoco existe
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

                                                    //aqui va la transiction 
                                                    var finalasssychart = _mapper.Map<AssyChart>(assychartForCreate);
                                                    dbContext.AssyCharts.Add(finalasssychart);
                                                    dbContext.SaveChanges();
                                                    Debug.WriteLine($"dbContext SaveChanges Succesfull ");

                                                    CountCreateAssycchart++;

                                                    Debug.WriteLine($"Create assychart id {finalasssychart.AssyChardId} plantid {(int)PathResume.PlantId} areaid {(int)PathResume.AreaId} distributionid {(int)PathResume.DistributionId} ");
                                                    AssyChartExist = finalasssychart;
                                                }

                                                string lastOperationName = null; // Para almacenar el último nombre de operación
                                                //Obtencion de los tiempos por renglon en base a operacion
                                                foreach (var row in rows.SkipWhile(r => r.RowNumber() < startingRow.RowNumber()))
                                                {
                                                    PathResume.OperationId = null;
                                                    // Obtener la celda en la columna B para cada renglón
                                                    var cellB = row.Cell("B");

                                                    // Verificar si la celda no está combinada y es mayor o igual a la fila 12
                                                    if (!cellB.IsMerged() && row.RowNumber() >= rowStartOperation)
                                                    {
                                                        var CellOpCode = row.Cell("C");
                                                        var CellOpDesc = row.Cell("D");
                                                        var CellCommentaryOrRestriction = row.Cell("E");

                                                        var ExcelOpCode = CellOpCode.Value.ToString() != "" ? CellOpCode.Value.ToString() : "";
                                                        var ExcelOpDescription = CellOpDesc.Value.ToString() != "" ? CellOpDesc.Value.ToString() : "";

                                                        if (!string.IsNullOrEmpty(ExcelOpDescription))
                                                        {
                                                            lastOperationName = ExcelOpDescription;
                                                        }

                                                        // Si no hay un nombre en la columna B, reutilizamos el último nombre de operación
                                                        if (lastOperationName != null)
                                                        {
                                                            ExcelOpDescription = lastOperationName;
                                                        }
                                                        var ExcelCommentaryOrRestriction = CellCommentaryOrRestriction.Value.ToString() != "" ? CellCommentaryOrRestriction.Value.ToString() : "";

                                                        if (string.IsNullOrEmpty(ExcelOpCode) && string.IsNullOrEmpty(ExcelOpDescription))
                                                        {
                                                            //si es renglon vacio brincamos al siguiente
                                                            continue;
                                                        }
                                                        else if (string.IsNullOrEmpty(ExcelOpCode) && !string.IsNullOrEmpty(ExcelOpDescription))
                                                        {
                                                            DocumentError = true;
                                                            eMailBody += $"\\n Falta No. Operacion..." +
                                                                $" Rango de celdas C{row.RowNumber()}" +
                                                                $" Pagina: {p} - {pageName}" +
                                                                $" Distribucion: {coincidenciasDistributions.Distribution.Description}";
                                                        }
                                                        else if (!string.IsNullOrEmpty(ExcelOpCode) && string.IsNullOrEmpty(ExcelOpDescription))
                                                        {
                                                            DocumentError = true;
                                                            eMailBody += $"\\n Falta Nombre de operacion..." +
                                                                $" Rango de celdas C{row.RowNumber()}" +
                                                                $" Pagina: {p} - {pageName}" +
                                                                $" Distribucion: {coincidenciasDistributions.Distribution.Description}";
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
                                                        //una copia de los productos vacios para añádir los tiempso correspodnientes
                                                        List<Dictionary<string, Dictionary<string, string>>> productsCopy = ObjectCloner.ObjectCloner.DeepClone(products);
                                                        for (int j = 0; j < productsCopy.Count; j++)
                                                        {
                                                            var product = productsCopy[j];
                                                            var productName = product.Keys.First();
                                                            product[productName]["Time"] = timeGroups[j];
                                                        }


                                                        // Eliminar productos sin tiempo de la copia
                                                        productsCopy = productsCopy.Where(product => product.Values.First()["Time"] != "§§§§").ToList();

                                                        //La operacion No existe, si la distribucion no existe hay que crearla
                                                        Debug.WriteLine($"La Operacion NO EXISTE {ExcelOpCode} - {ExcelOpDescription} NO Existe :c !!! ");
                                                        //creacion de json del producto con tiempos


                                                        if (productsCopy.Count > 0)
                                                        {
                                                            //Coincidencia de producto
                                                            string productCode = productsCopy[0].Keys.First();
                                                            //si es operacion de calidad añadimos el producto antes de cc
                                                            if (ExcelOpCode == "CC" || ExcelOpCode == "cc")
                                                            {
                                                                ExcelOpCode = $"{productCode} - {ExcelOpCode}";
                                                            }

                                                            if (ExcelOpCode.DiceCoefficient("FALTA GOS") > 0.8)
                                                            {
                                                                ExcelOpCode = $"{productCode} - {ExcelOpCode}";
                                                            }

                                                            var coincidenciasProduct = Products.Select(pair => new
                                                            {
                                                                Product = pair,
                                                                Similarity = 1 - pair.Code.JaccardDistance(productCode)
                                                            }).OrderByDescending(result => result.Similarity).FirstOrDefault();

                                                            // Ajusta este umbral según la necesidad
                                                            if (coincidenciasProduct != null && coincidenciasProduct.Similarity > 0.5)
                                                            {
                                                                PathResume.ProductID = coincidenciasProduct.Product.ProductId;
                                                            }

                                                            var finalproduct = coincidenciasProduct.Product;
                                                            Debug.WriteLine("GET product dbContext");

                                                            if (finalproduct != null)
                                                            {
                                                                if (finalproduct.Distributions != null)
                                                                {
                                                                    if (!finalproduct.Distributions.Any(d => d.DistributionId == finalDistribution.DistributionId))
                                                                        finalproduct.Distributions.Add(finalDistribution);
                                                                }
                                                                else
                                                                {
                                                                    finalproduct.Distributions = new List<Distribution>();
                                                                    finalproduct.Distributions.Add(finalDistribution);

                                                                }
                                                            }
                                                            await dbContext.SaveChangesAsync();

                                                            var ProductJson = productsCopy.FirstOrDefault(product => product.Keys.First() == productCode);

                                                            var operationForCreate = _mapper.Map<OperationForCreationDto>(new OperationForCreationDto() { Code = ExcelOpCode, Description = ExcelOpDescription, IsActive = true });
                                                            operationForCreate.restrictionorcomment = ExcelCommentaryOrRestriction;


                                                            operationForCreate.ProductName = productCode;
                                                            operationForCreate.NameTime = ProductJson.Values.First()["NameTime"];
                                                            operationForCreate.Time = ProductJson.Values.First()["Time"];
                                                            operationForCreate.AdditionalTime = ProductJson.Values.First()["AdditionalTime"];
                                                            operationForCreate.StandardTime = ProductJson.Values.First()["StandardTime"];


                                                            var finalOperation = _mapper.Map<Operation>(operationForCreate);

                                                            //aqui va la creacion de rutas
                                                            TreeItemData? mejorCoincidenciaHOE = null;
                                                            TreeItemData? mejorCoincidenciaGOS = null;
                                                            TreeItemData? mejorCoincidenciaCCP = null;

                                                            mejorCoincidenciaHOE = _treeService.EncontrarNodoMejorCoincidencia(rootNodeHOE, planta, "produccion", coincidenciasAreas.Area, coincidenciasDistributions.Distribution, coincidenciasProduct.Product);

                                                            if (mejorCoincidenciaHOE != null)
                                                            {
                                                                PathResume.HOE = mejorCoincidenciaHOE.Ruta;
                                                                Debug.WriteLine("HOE: " + mejorCoincidenciaHOE.Ruta);
                                                                Console.WriteLine("HOE: " + mejorCoincidenciaHOE.Ruta);
                                                            }

                                                            mejorCoincidenciaGOS = _treeService.EncontrarNodoMejorCoincidencia(rootNodeGOS, planta, "produccion", coincidenciasAreas.Area, coincidenciasDistributions.Distribution, coincidenciasProduct.Product);

                                                            if (mejorCoincidenciaGOS != null)
                                                            {
                                                                PathResume.GOS = mejorCoincidenciaGOS.Ruta;
                                                                Debug.WriteLine("GOS: " + mejorCoincidenciaGOS.Ruta);
                                                                Console.WriteLine("GOS: " + mejorCoincidenciaGOS.Ruta);
                                                            }

                                                            mejorCoincidenciaCCP = _treeService.EncontrarNodoMejorCoincidencia(rootNodeCCP, planta, "produccion", coincidenciasAreas.Area, coincidenciasDistributions.Distribution, coincidenciasProduct.Product);

                                                            if (mejorCoincidenciaCCP != null)
                                                            {
                                                                PathResume.CCP = mejorCoincidenciaCCP.Ruta;
                                                                Debug.WriteLine("CCP: " + mejorCoincidenciaCCP.Ruta);
                                                                Console.WriteLine("CCP: " + mejorCoincidenciaCCP.Ruta);
                                                            }


                                                            //assychart es nullo en este punto
                                                            SOSCodePath? ExistCodePath = await _context.CodePaths.Where(p => p.AssyChardId == AssyChartExist.AssyChardId && p.Code == finalOperation.Code).FirstOrDefaultAsync();

                                                            if (ExistCodePath is null)
                                                            {
                                                                // no existe se crea
                                                                //procedimiento de path
                                                                SOSCodePath CodePath = new SOSCodePath();

                                                                CodePath.Code = finalOperation.Code;


                                                                if (mejorCoincidenciaHOE != null)
                                                                {
                                                                    CodePath.HOE = mejorCoincidenciaHOE.Ruta;
                                                                }

                                                                if (mejorCoincidenciaGOS != null)
                                                                {
                                                                    CodePath.GOS = mejorCoincidenciaGOS.Ruta;
                                                                }

                                                                if (mejorCoincidenciaCCP != null)
                                                                {
                                                                    CodePath.CCP = mejorCoincidenciaCCP.Ruta;
                                                                }


                                                                //Añadimso distribucion y Producto

                                                                CodePath.DistributionId = (int)PathResume.DistributionId;
                                                                CodePath.ProductId = PathResume.ProductID;

                                                                CodePath.AssyChardId = AssyChartExist.AssyChardId;


                                                                ////Crear Code Path
                                                                //await _supervisorMobilityRepository.AssychartCreateCodePath(CodePath);

                                                                ////aqui se añade el path creado
                                                                //_supervisorMobilityRepository.AssychartAddCodePath(AssyChartExist, CodePath);
                                                                //await _supervisorMobilityRepository.SaveChangesAsync();

                                                                //Crear Code Path Version de using dbContext
                                                                dbContext.CodePaths.Add(CodePath);
                                                                //aqui se añade el path creado
                                                                if (AssyChartExist.RoutesProductsAssyChart != null)
                                                                {
                                                                    AssyChartExist.RoutesProductsAssyChart.Add(CodePath);
                                                                }
                                                                else
                                                                {
                                                                    AssyChartExist.RoutesProductsAssyChart = new List<SOSCodePath>();
                                                                    AssyChartExist.RoutesProductsAssyChart.Add(CodePath);
                                                                }
                                                                dbContext.SaveChanges();
                                                            }
                                                            else
                                                            {
                                                                //asignacion directa
                                                                if (mejorCoincidenciaHOE != null)
                                                                {
                                                                    ExistCodePath.HOE = mejorCoincidenciaHOE.Ruta;
                                                                }

                                                                if (mejorCoincidenciaGOS != null)
                                                                {
                                                                    ExistCodePath.GOS = mejorCoincidenciaGOS.Ruta;
                                                                }

                                                                if (mejorCoincidenciaCCP != null)
                                                                {
                                                                    ExistCodePath.CCP = mejorCoincidenciaCCP.Ruta;
                                                                }
                                                                if (ExistCodePath.DistributionId != coincidenciasDistributions.Distribution.DistributionId)
                                                                {
                                                                    ExistCodePath.DistributionId = (int)PathResume.DistributionId;
                                                                }
                                                                if (ExistCodePath.ProductId != coincidenciasProduct.Product.ProductId)
                                                                {
                                                                    ExistCodePath.ProductId = (int)PathResume.ProductID;
                                                                }

                                                                //la ruta ya existe se actualiza

                                                                //dbContext.CodePaths.Update(ExistCodePath);

                                                            }


                                                            var distribution = await _context.Distributions.Where(o => o.AreaId == (int)PathResume.AreaId && o.DistributionId == (int)PathResume.DistributionId).FirstOrDefaultAsync();
                                                            if (distribution != null)
                                                            {
                                                                distribution.Operations.Add(finalOperation);
                                                            }
                                                            dbContext.SaveChanges();

                                                            OperationsDictionary.Add(((int)PathResume.PlantId, (int)PathResume.AreaId, (int)PathResume.DistributionId, finalOperation.OperationId), finalOperation);
                                                            CountCreateOperation++;
                                                        }
                                                        else
                                                        {
                                                            //Debug.WriteLine($"Distribucio no existe y no hay productos ");
                                                            DocumentError = true;
                                                            eMailBody += $"\\n Faltan datos en el documento..." +
                                                              $" Rango de celdas F{row.RowNumber()}-Y{row.RowNumber()}" +
                                                              $" Pagina: {p} - {pageName}" +
                                                              $" Distribucion: {coincidenciasDistributions.Distribution.Description}";
                                                        }


                                                    }

                                                }

                                                //if (DocumentError)
                                                //{
                                                //    break;
                                                //}


                                            }

                                        }

                                        // Guardar cambios en cada iteración del ciclo
                                        await dbContext.SaveChangesAsync();


                                    }//for de paginas

                                }//end using


                            }//end try
                            catch (FileNotFoundException ex)
                            {
                                Console.WriteLine($"Error Tree Data: {ex.Message.ToString()}");
                                Debug.WriteLine($"Error Tree Data: {ex.Message.ToString()}");
                                DocumentError = true;
                                transaction.Rollback();
                                //no se pudo abrir el archivo
                            }//end trycatch to add excel to list
                            catch (Exception another)
                            {
                                Console.WriteLine($"Error Tree Data: {another.Message.ToString()}");
                                Debug.WriteLine($"Error Tree Data: {another.Message.ToString()}");
                                transaction.Rollback();
                            }



                            if (DocumentError)
                            {
                                // Algo salió mal en el ciclo, realiza un rollback de la transacción
                                if (_env.IsDevelopment())
                                {
                                    Debug.WriteLine($"Simulate Transaction.Rollback: _env.IsDevelopment() -> transaction.Commit()");
                                    Console.WriteLine($"Simulate Transaction.Rollback: _env.IsDevelopment() -> transaction.Commit()");
                                    transaction.Commit();
                                }
                                else
                                {
                                    transaction.Rollback();
                                }

                                //e-mail de errores 
                                int maxRetriesMail = 3; // Número máximo de intentos
                                TimeSpan retryIntervalMail = TimeSpan.FromSeconds(2); // Intervalo de tiempo entre intentos (2 segundos en este caso)
                                int retriesMail = 0;

                                while (retriesMail < maxRetriesMail)
                                {
                                    try
                                    {
                                        if (_env.IsDevelopment())
                                        {
                                            Debug.WriteLine($"Email Simulado Enviado: _env.IsDevelopment() ");
                                        }
                                        else
                                        {
                                            var emailMessage = _email.CreateEmailMessage(userEntity.Email, eMailSubject, eMailBody);
                                            _email.Send(emailMessage);
                                        }
                                        Debug.WriteLine($"e-mail de errores enviado");
                                        break;
                                    }
                                    catch (Exception exceptionMail)
                                    {

                                        // Maneja la excepción aquí, si es necesario
                                        Debug.WriteLine($"Fallo Enviar mail: {exceptionMail.Message}");

                                        // Incrementa el número de intentos
                                        retriesMail++;

                                        // Espera el intervalo de tiempo antes de volver a intentarlo
                                        await Task.Delay(retryIntervalMail);
                                    }

                                }

                                // Notificacion de Errores

                                int maxIntentos = 5; // Número máximo de intentos
                                TimeSpan newintentTime = TimeSpan.FromSeconds(5); // Intervalo de tiempo entre intentos (5 segundos en este caso)
                                int intentos = 0;

                                while (intentos < maxIntentos)
                                {
                                    try
                                    {

                                        Notification NotyFinish = new Notification();
                                        NotyFinish.NotificationType = $"Error: PlantStructureData Procces {DateTime.Now}";
                                        eMailBody += " it is necessary to make the necessary corrections to the document and try to upload it again.";
                                        NotyFinish.NotificationText = $"{eMailBody}";

                                        NotyFinish.MadeBy = "PlantStructureData Process System";
                                        NotyFinish.UserId = userEntity.UserId;
                                        NotyFinish.IsAccepted = true;
                                        NotyFinish.IsActive = true;
                                        NotyFinish.EntryDate = DateTime.Now;

                                        _supervisorMobilityRepository.AddNotificationAsync(NotyFinish);
                                        await _supervisorMobilityRepository.SaveChangesAsync();
                                        Debug.WriteLine($"Notificacion de errores enviado");

                                        break;
                                    }
                                    catch (Exception exceptionNotify)
                                    {
                                        intentos++;
                                        Debug.WriteLine($"Fallo crear error Notification: {exceptionNotify.Message}");
                                        await Task.Delay(newintentTime);
                                    }

                                }
                            }
                            else
                            {
                                // Confirmacion de la transacción si todo bien
                                transaction.Commit();
                                int maxRetriesMail = 5; // Número máximo de intentos
                                TimeSpan retryIntervalMail = TimeSpan.FromSeconds(5); // Intervalo de tiempo entre intentos (5 segundos en este caso)
                                int retriesMail = 0;

                                while (retriesMail < maxRetriesMail)
                                {
                                    try
                                    {
                                        if (_env.IsDevelopment())
                                        {
                                            Debug.WriteLine($"Email Simulado Enviado: _env.IsDevelopment() ");
                                        }
                                        else
                                        {
                                            var emailMessage = _email.CreateEmailMessage(userEntity.Email, eMailSubject, eMailBody);
                                            _email.Send(emailMessage);
                                        }
                                        break;
                                    }
                                    catch (Exception ex)
                                    {
                                        retriesMail++;
                                        Debug.WriteLine($"Fallo Enviar mail: {ex.Message}");
                                        await Task.Delay(retryIntervalMail);
                                    }

                                }

                                // notificacion
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
                                        await _supervisorMobilityRepository.SaveChangesAsync();
                                        break;
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.WriteLine($"Fallo crear Succes Notification: {ex.Message}");
                                        intentos++;
                                        await Task.Delay(newintentTime);
                                    }

                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                            // Algo salió mal en el ciclo, realiza un rollback de la transacción
                            if (_env.IsDevelopment())
                            {
                                Debug.WriteLine($"Simulate Transaction.Rollback: _env.IsDevelopment() -> transaction.Commit()");
                                Console.WriteLine($"Simulate Transaction.Rollback: _env.IsDevelopment() -> transaction.Commit()");
                                transaction.Commit();
                            }
                            else
                            {
                                transaction.Rollback();
                            }

                            // Puedes registrar el error o realizar otras acciones necesarias
                            //e-mail de errores 
                            int maxRetriesMail = 3; // Número máximo de intentos
                            TimeSpan retryIntervalMail = TimeSpan.FromSeconds(2); // Intervalo de tiempo entre intentos (2 segundos en este caso)
                            int retriesMail = 0;

                            while (retriesMail < maxRetriesMail)
                            {
                                try
                                {
                                    if (_env.IsDevelopment())
                                    {
                                        Debug.WriteLine($"Email Simulado Enviado: _env.IsDevelopment() ");
                                    }
                                    else
                                    {
                                        var emailMessage = _email.CreateEmailMessage(userEntity.Email, eMailSubject, eMailBody);
                                        _email.Send(emailMessage);
                                    }
                                    Debug.WriteLine($"e-mail de errores enviado");
                                    break;
                                }
                                catch (Exception exceptionMail)
                                {

                                    // Maneja la excepción aquí, si es necesario
                                    Debug.WriteLine($"Fallo Enviar mail: {exceptionMail.Message}");

                                    // Incrementa el número de intentos
                                    retriesMail++;

                                    // Espera el intervalo de tiempo antes de volver a intentarlo
                                    await Task.Delay(retryIntervalMail);
                                }

                            }

                            // Notificacion de Errores

                            int maxIntentos = 5; // Número máximo de intentos
                            TimeSpan newintentTime = TimeSpan.FromSeconds(5); // Intervalo de tiempo entre intentos (5 segundos en este caso)
                            int intentos = 0;

                            while (intentos < maxIntentos)
                            {
                                try
                                {

                                    Notification NotyFinish = new Notification();
                                    NotyFinish.NotificationType = $"Error: PlantStructureData Procces {DateTime.Now}";
                                    eMailBody += " it is necessary to make the necessary corrections to the document and try to upload it again.";
                                    NotyFinish.NotificationText = $"{eMailBody}";

                                    NotyFinish.MadeBy = "PlantStructureData Process System";
                                    NotyFinish.UserId = userEntity.UserId;
                                    NotyFinish.IsAccepted = true;
                                    NotyFinish.IsActive = true;
                                    NotyFinish.EntryDate = DateTime.Now;

                                    _supervisorMobilityRepository.AddNotificationAsync(NotyFinish);
                                    break;
                                }
                                catch (Exception exceptionNotify)
                                {
                                    intentos++;
                                    Debug.WriteLine($"Fallo crear error Notification: {exceptionNotify.Message}");
                                    await Task.Delay(newintentTime);
                                }

                            }


                        }//end catch


                    }//transaction
                }//end db context

            }//end using scope
        }

    }//end class ProcessWorkLoadService
}//end namespace
