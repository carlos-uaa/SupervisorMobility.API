using AutoMapper;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using FuzzyString;
using Microsoft.IdentityModel.Tokens;
using Slugify;
using SupervisorMobility.API.Business;
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

namespace SupervisorMobility.API.DataAccess.Services
{
    public class BackgroundProcessingService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public string _fileName = string.Empty;
        public int _userId = 0;
        public int _plantId = 0;
        public int _option = 0;

        public BackgroundProcessingService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task StartAsync(string fileName, int userId, int option, CancellationToken stoppingToken, int plantname = 0)
        {
            _fileName = fileName;
            _userId = userId;
            _option = option;
            _plantId = plantname;
            await StartAsync(CancellationToken.None);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Tu lógica de procesamiento en segundo plano aquí
            switch (_option)
            {
                case 1:
                    await ProcessDocumentHeadCountAsync(_fileName, _userId, stoppingToken);
                    break;
                case 2:
                    await ProcessTreeDataAsync(_fileName, _plantId, _userId, stoppingToken);
                    break;
                case 3:
                    await ProcessPathsAsync(_fileName, _userId, stoppingToken);
                    break;
            }

        }


        private async Task ProcessDocumentHeadCountAsync(string trustedFileNameForStorage, int UserIdUpload, CancellationToken stoppingToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;

                var _supervisorMobilityRepository = serviceProvider.GetRequiredService<ISupervisorMobilityRepository>();
                var _email = serviceProvider.GetRequiredService<IEmailService>();

                await _supervisorMobilityRepository.RemoveAllHeadCountRegisters();

                User userEntity = await _supervisorMobilityRepository.GetUserAsync(UserIdUpload, false);

                //Start Massive Upload 
                string filepath = Directory.GetCurrentDirectory().ToString() + "\\uploads\\headcount\\" + trustedFileNameForStorage;
                string messageError = "";
                try
                {
                    using (var workBook = new XLWorkbook(filepath))
                    {
                        var pages = workBook.Worksheets.Count - 1;

                        //for (int p = 1; p <= pages; p++)
                        //{
                        IXLWorksheet ws = workBook.Worksheet(1);


                        bool firstRow = true;
                        int i = 1;
                        foreach (IXLRow row in ws.Rows())
                        {
                            //Use the first row to add columns to DataTable.
                            HeadCount _headCount = new HeadCount();

                            if (firstRow)
                            {
                                firstRow = false;
                            }
                            else
                            {
                                if (!row.IsEmpty())
                                {
                                    int maxRetries = 5; // Número máximo de intentos
                                    TimeSpan retryInterval = TimeSpan.FromSeconds(5); // Intervalo de tiempo entre intentos (5 segundos en este caso)
                                    int retries = 0;

                                    while (retries < maxRetries)
                                    {
                                        try
                                        {


                                            try
                                            {
                                                // id subarea nombre subarea
                                                var valueFunctionDescription = ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 5).GetString() != "" ? ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 5).GetValue<string>().Trim() : "";


                                                bool contieneNumero = valueFunctionDescription.Any(char.IsDigit);

                                                //la celda esta dentro de los preocesso
                                                if (contieneNumero)
                                                {
                                                    //Tiene id de subarea,  extraemos numero
                                                    string numeroString = new string(valueFunctionDescription.Where(char.IsDigit).ToArray());

                                                    //convertimos
                                                    if (int.TryParse(numeroString, out int numero))
                                                    {
                                                        //guaramos id
                                                        _headCount.ID_subarea = numero;
                                                    }
                                                    else
                                                    {
                                                        //fallo el numero asignamos default
                                                        _headCount.ID_subarea = 0;
                                                    }
                                                }
                                                else
                                                {
                                                    //No tiene id de subarea
                                                    _headCount.ID_subarea = 0;
                                                }

                                                try
                                                {
                                                    _headCount.nombre_subarea = valueFunctionDescription;
                                                }
                                                catch (Exception ex)
                                                {

                                                }

                                                try
                                                {
                                                    _headCount.Fuction_Type = ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 16).GetString() != "" ? ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 16).GetValue<string>().Trim() : "";
                                                }
                                                catch (Exception ex)
                                                {

                                                }

                                                //break;


                                            }
                                            catch (Exception ex)
                                            {

                                            }

                                            try
                                            {
                                                _headCount.RTO = ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 9).GetString() != "" ? ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 9).GetValue<string>() : "";

                                            }
                                            catch (Exception ex)
                                            {

                                            }

                                            //procedimiento
                                            try
                                            {
                                                _headCount.Codigo = ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 1).GetString() != "" ? (int)ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 1).Value : -1;
                                            }
                                            catch (Exception ex)
                                            {

                                            }
                                            try
                                            {
                                                _headCount.CO = ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 2).GetString() != "" ? ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 2).GetValue<string>() : "";
                                                //                                  ToInsertIntoList.GOS = ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 3).GetString() != "" ? ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 3).GetValue<string>() : "";
                                            }
                                            catch (Exception ex)
                                            {

                                            }
                                            try
                                            {
                                                var valuesArea = ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 3).GetString() != "" ? ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 3).GetValue<string>() : "";
                                                var splitedArea = valuesArea.Split("-");

                                                try
                                                {
                                                    _headCount.ID_Area = int.Parse(splitedArea[0]);
                                                }
                                                catch (Exception ex)
                                                {

                                                }

                                                try
                                                {
                                                    _headCount.Nombre_Area = splitedArea[1];
                                                }
                                                catch (Exception ex)
                                                {

                                                }


                                            }
                                            catch (Exception ex)
                                            {

                                            }

                                            try
                                            {

                                                var valueDepartament = ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 4).GetString() != "" ? ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 4).GetValue<string>() : "";

                                                if (valueDepartament.Contains("_") && valueDepartament.Contains("-"))
                                                {
                                                    var CostDepartament = valueDepartament.Split("_");
                                                    var splitedDepartament = CostDepartament[0].Split("-");
                                                    try
                                                    {
                                                        _headCount.Cost_center = int.Parse(splitedDepartament[0]);
                                                    }
                                                    catch (Exception ex)
                                                    {

                                                    }
                                                    try
                                                    {
                                                        _headCount.ID_Departamento = splitedDepartament[1];
                                                    }
                                                    catch (Exception ex)
                                                    {

                                                    }
                                                    try
                                                    {
                                                        _headCount.Nombre_Departamento = CostDepartament[1];
                                                    }
                                                    catch (Exception ex)
                                                    {

                                                    }
                                                }
                                                else if (!valueDepartament.Contains("_") && valueDepartament.Contains("-"))
                                                {
                                                    var firstSplit = valueDepartament.Split("-");
                                                    try
                                                    {
                                                        _headCount.Cost_center = int.Parse(firstSplit[0]);
                                                    }
                                                    catch (Exception ex)
                                                    {

                                                    }
                                                    try
                                                    {
                                                        _headCount.ID_Departamento = firstSplit[1];
                                                    }
                                                    catch (Exception ex)
                                                    {

                                                    }
                                                    try
                                                    {
                                                        _headCount.Nombre_Departamento = firstSplit[1];
                                                    }
                                                    catch (Exception ex)
                                                    {

                                                    }
                                                }
                                                else if (valueDepartament.Contains("_") && !valueDepartament.Contains("-"))
                                                {
                                                    var firstSplit2 = valueDepartament.Split("_");

                                                    if (int.TryParse(firstSplit2[0], out int numero))
                                                    {
                                                        //guaramos id
                                                        _headCount.Cost_center = numero;
                                                    }
                                                    else
                                                    {
                                                        //fallo el numero asignamos default
                                                        _headCount.Cost_center = 0;
                                                    }

                                                    try
                                                    {
                                                        _headCount.ID_Departamento = firstSplit2[1];
                                                    }
                                                    catch (Exception ex)
                                                    {

                                                    }
                                                    try
                                                    {
                                                        _headCount.Nombre_Departamento = firstSplit2[1];
                                                    }
                                                    catch (Exception ex)
                                                    {

                                                    }

                                                }



                                            }
                                            catch (Exception ex)
                                            {

                                            }


                                            try
                                            {
                                                _headCount.Nivel = ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 6).GetString() != "" ? ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 6).GetValue<string>() : "";

                                            }
                                            catch (Exception ex)
                                            {

                                            }
                                            try
                                            {
                                                _headCount.Group = ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 7).GetString() != "" ? ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 7).GetValue<string>() : "";
                                            }
                                            catch (Exception ex)
                                            {

                                            }
                                            try
                                            {
                                                _headCount.BUDGET = ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 8).GetString() != "" ? ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 8).GetValue<string>() : "";
                                            }
                                            catch (Exception ex)
                                            {

                                            }

                                            try
                                            {
                                                var valueHC = ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 1).GetString() != "" ? ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 1).Value.ToString() : "";
                                                try
                                                {
                                                    _headCount.HC = int.Parse(valueHC);
                                                }
                                                catch (Exception ex)
                                                {

                                                }
                                            }
                                            catch (Exception ex)
                                            {

                                            }
                                            try
                                            {
                                                _headCount.Comentarios = ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 11).GetString() != "" ? ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 11).GetValue<string>() : "";
                                            }
                                            catch (Exception ex)
                                            {

                                            }
                                            try
                                            {
                                                _headCount.LABOR_TYPE = ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 12).GetString() != "" ? ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 12).GetValue<string>() : "";
                                            }
                                            catch (Exception ex)
                                            {

                                            }
                                            try
                                            {
                                                _headCount.Fecha_de_alta = DateTime.Now;
                                            }
                                            catch (Exception ex)
                                            {

                                            }
                                            try
                                            {
                                                _headCount.UserUploadId = UserIdUpload;
                                            }
                                            catch (Exception ex)
                                            {
                                            }
                                            try
                                            {
                                                _headCount.Usuario_de_alta = userEntity.Name;
                                            }
                                            catch (Exception ex)
                                            {

                                            }

                                            await _supervisorMobilityRepository.AddHeadCoutAsync(_headCount);


                                            retries = 0;

                                            Debug.WriteLine($"Intento {retries + 1} Linea Position [{i}]");

                                            // Si la operación tiene éxito, puedes salir del bucle
                                            break;
                                        }
                                        catch (Exception ex)
                                        {

                                            // Maneja la excepción aquí, si es necesario
                                            Debug.WriteLine($"Intento {retries + 1} Linea Position [{i}] falló: {ex.Message}");

                                            // Incrementa el número de intentos
                                            retries++;


                                            if (retries == 5)
                                            {
                                                //añade notificacion de error
                                                messageError += $"Error in data ROW [{i}], please check document and solve this issue \n, ";

                                            }

                                            // Espera el intervalo de tiempo antes de volver a intentarlo
                                            await Task.Delay(retryInterval);
                                        }



                                    }//While

                                }//end is not empety row
                            }//end else first roe
                            i++;
                        }//end foreach

                        //}//for de paginas

                    }//end using woorkbook



                }//end try
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error en Using Woorkbook {ex.ToString()}");
                }//end trycatch to add excel to list

                int maxRetriesMail = 2; // Número máximo de intentos
                TimeSpan retryIntervalMail = TimeSpan.FromSeconds(5); // Intervalo de tiempo entre intentos (5 segundos en este caso)
                int retriesMail = 0;

                //while (retriesMail < maxRetriesMail)
                //{
                try
                {


                    if (!messageError.IsNullOrEmpty())
                    {
                        var emailMessageError = _email.CreateEmailMessage(userEntity.Email, "Headcount processed", $"Headcount document has been processed, you can now review its contents on the details page. \n LIST ERRORS:  \n" + messageError);
                        _email.Send(emailMessageError);
                    }
                    else
                    {
                        var emailMessage = _email.CreateEmailMessage(userEntity.Email, "Headcount processed", $"Headcount document has been processed, you can now review its contents on the details page.");
                        _email.Send(emailMessage);
                    }
                    //break;
                }
                catch (Exception ex)
                {

                    // Maneja la excepción aquí, si es necesario
                    Debug.WriteLine($"Fallo send Succes e-mail: {ex.Message}");

                    // Incrementa el número de intentos
                    retriesMail++;

                    // Espera el intervalo de tiempo antes de volver a intentarlo
                    await Task.Delay(retryIntervalMail);

                    Notification NotyError = new Notification();
                    NotyError.NotificationType = $"HeadCount Error Succes e-mail {DateTime.Now}";
                    NotyError.NotificationText = messageError;

                    NotyError.MadeBy = "HeadCount System";
                    NotyError.UserId = userEntity.UserId;
                    NotyError.IsAccepted = true;
                    NotyError.IsActive = true;
                    NotyError.EntryDate = DateTime.Now;

                    _supervisorMobilityRepository.AddNotificationAsync(NotyError);
                }

                //}

                // notificacion
                //añade notificacion de error

                int maxIntentos = 3; // Número máximo de intentos
                TimeSpan newintentTime = TimeSpan.FromSeconds(5); // Intervalo de tiempo entre intentos (5 segundos en este caso)
                int intentos = 0;

                while (intentos < maxIntentos)
                {
                    try
                    {
                        if (!messageError.IsNullOrEmpty())
                        {


                            Notification NotyFinish = new Notification();
                            NotyFinish.NotificationType = $"HeadCount Procces - Succes With Errors  {DateTime.Now}";
                            NotyFinish.NotificationText = $"Headcount document has been processed, you can now review its contents on the details page. \n LIST ERRORS:  \n" + messageError;

                            NotyFinish.MadeBy = "HeadCount Process System ";
                            NotyFinish.UserId = userEntity.UserId;
                            NotyFinish.IsAccepted = true;
                            NotyFinish.IsActive = true;
                            NotyFinish.EntryDate = DateTime.Now;

                            _supervisorMobilityRepository.AddNotificationAsync(NotyFinish);
                        }
                        else
                        {
                            Notification NotyFinish = new Notification();
                            NotyFinish.NotificationType = $"HeadCount Procces - Succes  {DateTime.Now}";
                            NotyFinish.NotificationText = $"Headcount document has been processed, you can now review its contents on the details page.";

                            NotyFinish.MadeBy = "HeadCount Process System ";
                            NotyFinish.UserId = userEntity.UserId;
                            NotyFinish.IsAccepted = true;
                            NotyFinish.IsActive = true;
                            NotyFinish.EntryDate = DateTime.Now;

                            _supervisorMobilityRepository.AddNotificationAsync(NotyFinish);
                        }



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
                            NotyError.NotificationType = $"HeadCount Finish: {DateTime.Now}";
                            NotyError.NotificationText = $"Finish procces document";

                            NotyError.MadeBy = "HeadCount System";
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


            }//end ussing scope

        }

        public async Task ProcessTreeDataAsync(string trustedFileNameForStorage, int plantnameid, int UserIdUpload, CancellationToken stoppingToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;

                var _supervisorMobilityRepository = serviceProvider.GetRequiredService<ISupervisorMobilityRepository>();
                var _assyChartService = serviceProvider.GetRequiredService<IAssyChartService>();
                var _email = serviceProvider.GetRequiredService<IEmailService>();
                var _mapper = serviceProvider.GetRequiredService<IMapper>();
                var _treeService = serviceProvider.GetRequiredService<ITreeService>();
                var customHttp = serviceProvider.GetRequiredService<CustomHttpClientService>();
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
                string filepath = Directory.GetCurrentDirectory().ToString() + "\\uploads\\massive\\" + trustedFileNameForStorage;
                try
                {
                    using (var workBook = new XLWorkbook(filepath))
                    {
                        var pages = workBook.Worksheets.Count;
                        int CountCreateAssycchart = 0;

                        for (int p = 1; p <= pages; p++)
                        {
                            int CountCreateOperation = 0;
                            IXLWorksheet ws = workBook.Worksheet(p);


                            //La optencion del producto ira dada por los tiempos

                            //var productCode = ws.Name;
                            //Debug.WriteLine($"Product Name: {productCode}");

                            //var ProductExist = Products.Select(pair => new
                            //{
                            //    Product = pair,
                            //    Similarity = 1 - pair.Code.JaccardDistance(productCode)
                            //}).OrderByDescending(result => result.Similarity).FirstOrDefault();

                            string pageName = ws.Name;

                            var cellAddress = "B6";
                            IXLCell targetCell = ws.Cell(cellAddress);

                            if (targetCell.IsMerged())
                            {
                                // La celda está combinada, puedes obtener su contenido
                                var mergedRange = ws.MergedRanges.FirstOrDefault(r => r.Contains(targetCell.Address.ToString()));
                                if (mergedRange != null)
                                {
                                    Console.WriteLine($"La celda {cellAddress} está combinada. Contenido: {mergedRange.FirstCell().Value}");

                                    // También puedes obtener todas las celdas combinadas
                                    var combinedCells = mergedRange.CellsUsed().Select(c => c.Address.ToString());
                                    Console.WriteLine($"Celdas combinadas: {string.Join(", ", combinedCells)}");
                                }
                            }
                            else
                            {
                                // La celda no está combinada
                                Console.WriteLine($"La celda {cellAddress} no está combinada. Contenido: {targetCell.Value}");
                            }


                            if (userEntity.UserType == 2)
                            {
                                //Comprobacion de que la carga esta pertenece a alguna de las areas
                                bool noAreaAssigned = true;




                                if(p == 1 && !noAreaAssigned)
                                {
                                    //incluimos asunto del correo 

                                }
                            }
                            else if(userEntity.UserType == 3) {
                                //Comprobacion, la carga no pertenece a su area
                            }


                            //bool firstRow = true;
                            //int i = 2;
                            //foreach (IXLRow row in ws.Rows())
                            //{
                            //    //Use the first row to add columns to DataTable.

                            //    if (firstRow)
                            //    {
                            //        firstRow = false;
                            //    }
                            //    else
                            //    {
                            //        if (!row.IsEmpty())
                            //        {

                            //            PathInfo PathResume = new PathInfo();

                            //            int maxRetries = 5; // Número máximo de intentos
                            //            TimeSpan retryInterval = TimeSpan.FromSeconds(5); // Intervalo de tiempo entre intentos (5 segundos en este caso)
                            //            int retries = 0;

                            //            while (retries < maxRetries)
                            //            {
                            //                try
                            //                {

                            //                    //if (p == 1)
                            //                    //{
                            //                    var ExcelOpCode = ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 1).Value.ToString() != "" ? ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 1).Value.ToString() : "";
                            //                    var ExcelOpDescription = ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 5).Value.ToString() != "" ? ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 5).Value.ToString() : "";

                            //                    var ExcelAreaCode = ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 3).Value.ToString() != "" ? ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 3).Value.ToString() : "";
                            //                    var ExcelAreaDescription = ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 4).Value.ToString() != "" ? ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 4).Value.ToString() : "";

                            //                    var ExcelDistDescription = ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 6).Value.ToString() != "" ? ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 6).Value.ToString() : "";

                            //                    if (ExcelOpCode.IsNullOrEmpty() && ExcelOpDescription.IsNullOrEmpty() &&
                            //                        ExcelAreaCode.IsNullOrEmpty() && ExcelAreaDescription.IsNullOrEmpty() && ExcelDistDescription.IsNullOrEmpty())
                            //                    {
                            //                        break;
                            //                    }
                            //                    else if (ExcelAreaCode.IsNullOrEmpty() && ExcelAreaDescription.IsNullOrEmpty())
                            //                    {
                            //                        break;
                            //                    }
                            //                    else if (ExcelAreaCode.IsNullOrEmpty() && ExcelDistDescription.IsNullOrEmpty())
                            //                    {
                            //                        break;
                            //                    }

                            //                    if (!ExcelDistDescription.IsNullOrEmpty())
                            //                    {
                            //                        auxDistribution = ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 6).Value.ToString() != "" ? ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 6).Value.ToString() : "";
                            //                    }
                            //                    else if (ExcelDistDescription == "")
                            //                    {
                            //                        ExcelDistDescription = auxDistribution;
                            //                    }


                            //                    if (ProductExist != null && ProductExist.Similarity > 0.5) // Ajusta este umbral según tus necesidades
                            //                    {
                            //                        PathResume.ProductID = ProductExist.Product.ProductId;
                            //                    }

                            //                    // Buscar el ID de planta en el diccionario de plantas
                            //                    var planta = PlantsDictionary.Values.FirstOrDefault(p => p.PlantId == plantnameid);
                            //                    if (planta != null)
                            //                    {
                            //                        PathResume.PlantId = planta.PlantId;
                            //                    }
                            //                    else
                            //                    {
                            //                        //mensaje de error
                            //                        return;
                            //                    }


                            //                    if (PathResume.PlantId > 0)
                            //                    {// Buscar coincidencia en area
                            //                        var coincidenciasAreas = AreasDictionary
                            //                               .Where(pair => pair.Key.Item1 == PathResume.PlantId) // Filtramos por ID de planta
                            //                               .Select(pair => new
                            //                               {
                            //                                   Area = pair.Value,
                            //                                   Similarity = pair.Value.Code.Equals(ExcelAreaCode)
                            //                                       ? 1.0 // Si los códigos coinciden exactamente, la similitud es máxima
                            //                                       : 1 - pair.Value.Code.JaccardDistance(ExcelAreaCode)
                            //                               })
                            //                               .OrderByDescending(result => result.Similarity)
                            //                               .FirstOrDefault();

                            //                        if (coincidenciasAreas != null && coincidenciasAreas.Similarity >= 0.70) // Ajusta este umbral según tus necesidades
                            //                        {
                            //                            PathResume.AreaId = coincidenciasAreas.Area.AreaId;
                            //                            PathResume.DescripcionArea = coincidenciasAreas.Area.Description;
                            //                        }


                            //                        if (PathResume.AreaId > 0)
                            //                        {
                            //                            // Buscar coincidencia en distribucion
                            //                            var coincidenciasDistributions = DistributionsDictionary
                            //                                .Where(pair => pair.Key.Item1 == PathResume.PlantId && pair.Key.Item2 == PathResume.AreaId)
                            //                                .Select(pair => new
                            //                                {
                            //                                    Distribution = pair.Value,
                            //                                    Similarity = 1 - pair.Value.Description.JaccardDistance(ExcelDistDescription)
                            //                                })
                            //                                .OrderByDescending(result => result.Similarity)
                            //                                .FirstOrDefault();

                            //                            if (coincidenciasDistributions != null && coincidenciasDistributions.Similarity > 0.5)
                            //                            {
                            //                                PathResume.DistributionId = coincidenciasDistributions.Distribution.DistributionId;
                            //                                PathResume.DescripcionDistribucion = coincidenciasDistributions.Distribution.Description;
                            //                            }


                            //                            if (PathResume.DistributionId > 0)
                            //                            {

                            //                                var coincidenciasOperaciones = OperationsDictionary
                            //                               .Where(pair => pair.Key.Item1 == PathResume.PlantId && pair.Key.Item2 == PathResume.AreaId && pair.Key.Item3 == PathResume.DistributionId)
                            //                               .Select(pair => new
                            //                               {
                            //                                   Operation = pair.Value,
                            //                                   Similarity = (pair.Value.Code == ExcelOpCode && pair.Value.Description == ExcelOpDescription ? 1 : 0)
                            //                               })
                            //                               .OrderByDescending(result => result.Similarity)
                            //                               .FirstOrDefault();

                            //                                if (coincidenciasOperaciones != null && coincidenciasOperaciones.Similarity > 0.5)
                            //                                {
                            //                                    PathResume.OperationId = coincidenciasOperaciones.Operation.OperationId;
                            //                                }

                            //                                if (PathResume.OperationId > 0)
                            //                                {
                            //                                    //existe no se hace nada
                            //                                }
                            //                                else
                            //                                {//No existe hay que crearla
                            //                                    var operationForCreate = _mapper.Map<OperationForCreationDto>(new OperationForCreationDto() { Code = ExcelOpCode, Description = ExcelOpDescription, IsActive = true });
                            //                                    var finalOperation = _mapper.Map<Operation>(operationForCreate);
                            //                                    await _supervisorMobilityRepository.AddOperationForDistributionAsync((int)PathResume.AreaId, (int)PathResume.DistributionId, finalOperation);
                            //                                    await _supervisorMobilityRepository.SaveChangesAsync();

                            //                                    OperationsDictionary.Add(((int)PathResume.PlantId, (int)PathResume.AreaId, (int)PathResume.DistributionId, finalOperation.OperationId), finalOperation);
                            //                                    CountCreateOperation++;
                            //                                }


                            //                            }//end if distribution >0
                            //                            else
                            //                            {
                            //                                //distribution no existe- se crea todo
                            //                                string codeGen = ExcelDistDescription;

                            //                                SlugHelper slugHelper = new SlugHelper();
                            //                                string slug = slugHelper.GenerateSlug(codeGen);

                            //                                var distributionForCreate = _mapper.Map<DistributionForCreationDto>(new DistributionForCreationDto() { Code = slug, Description = ExcelDistDescription, IsActive = true });
                            //                                var finalDistribution = _mapper.Map<Distribution>(distributionForCreate);
                            //                                await _supervisorMobilityRepository.AddDistributionForPlantAsync((int)PathResume.PlantId, (int)PathResume.AreaId, finalDistribution);
                            //                                await _supervisorMobilityRepository.SaveChangesAsync();
                            //                                PathResume.DistributionId = finalDistribution.DistributionId;
                            //                                DistributionsDictionary.Add(((int)PathResume.PlantId, (int)PathResume.AreaId, (int)PathResume.DistributionId), finalDistribution);

                            //                                await _supervisorMobilityRepository.AddDistributionForProductAsync((int)PathResume.ProductID, finalDistribution);

                            //                                //la operacion no existira

                            //                                var operationForCreate = _mapper.Map<OperationForCreationDto>(new OperationForCreationDto() { Code = ExcelOpCode, Description = ExcelOpDescription, IsActive = true });
                            //                                var finalOperation = _mapper.Map<Operation>(operationForCreate);
                            //                                await _supervisorMobilityRepository.AddOperationForDistributionAsync((int)PathResume.AreaId, (int)PathResume.DistributionId, finalOperation);
                            //                                await _supervisorMobilityRepository.SaveChangesAsync();

                            //                                OperationsDictionary.Add(((int)PathResume.PlantId, (int)PathResume.AreaId, (int)PathResume.DistributionId, finalOperation.OperationId), finalOperation);

                            //                                CountCreateOperation++;

                            //                            }//end else distribuccion no existe
                            //                        }//end if area > 0
                            //                        else
                            //                        {
                            //                            //area no existe- se crea todo
                            //                            var areaForCreate = _mapper.Map<AreaForCreationDto>(new AreaForCreationDto() { Code = ExcelAreaCode, Description = ExcelAreaDescription, IsActive = true });

                            //                            var finalArea = _mapper.Map<Area>(areaForCreate);
                            //                            finalArea.PlantId = (int)PathResume.PlantId;

                            //                            await _supervisorMobilityRepository.AddArea(finalArea);
                            //                            await _supervisorMobilityRepository.AddAreaForPlantAsync((int)PathResume.PlantId, finalArea);
                            //                            await _supervisorMobilityRepository.SaveChangesAsync();
                            //                            PathResume.AreaId = finalArea.AreaId;

                            //                            AreasDictionary.Add(((int)PathResume.PlantId, (int)PathResume.AreaId), finalArea);

                            //                            //la distribuccion no existira
                            //                            string codeGen = ExcelDistDescription;

                            //                            SlugHelper slugHelper = new SlugHelper();
                            //                            string slug = slugHelper.GenerateSlug(codeGen);

                            //                            var distributionForCreate = _mapper.Map<DistributionForCreationDto>(new DistributionForCreationDto() { Code = slug, Description = ExcelDistDescription, IsActive = true });
                            //                            var finalDistribution = _mapper.Map<Distribution>(distributionForCreate);
                            //                            await _supervisorMobilityRepository.AddDistributionForPlantAsync((int)PathResume.PlantId, (int)PathResume.AreaId, finalDistribution);
                            //                            await _supervisorMobilityRepository.SaveChangesAsync();
                            //                            PathResume.DistributionId = finalDistribution.DistributionId;

                            //                            DistributionsDictionary.Add(((int)PathResume.PlantId, (int)PathResume.AreaId, (int)PathResume.DistributionId), finalDistribution);
                            //                            await _supervisorMobilityRepository.AddDistributionForProductAsync((int)PathResume.ProductID, finalDistribution);

                            //                            //la operacion no existira

                            //                            var operationForCreate = _mapper.Map<OperationForCreationDto>(new OperationForCreationDto() { Code = ExcelOpCode, Description = ExcelOpDescription, IsActive = true });
                            //                            var finalOperation = _mapper.Map<Operation>(operationForCreate);
                            //                            await _supervisorMobilityRepository.AddOperationForDistributionAsync((int)PathResume.AreaId, (int)PathResume.DistributionId, finalOperation);
                            //                            await _supervisorMobilityRepository.SaveChangesAsync();

                            //                            OperationsDictionary.Add(((int)PathResume.PlantId, (int)PathResume.AreaId, (int)PathResume.DistributionId, finalOperation.OperationId), finalOperation);
                            //                            CountCreateOperation++;

                            //                        }

                            //                    }//end if plant >0
                            //                     // no hay chance de que la planta no exista


                            //                    //}
                            //                    //else
                            //                    //{
                            //                    //    //paginas siguientes
                            //                    //    var ExcelOpCode = ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 1).Value.ToString() != "" ? ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 1).Value.ToString() : "";
                            //                    //    var ExcelOpDescription = ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 4).Value.ToString() != "" ? ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 4).Value.ToString() : "";

                            //                    //    var ExcelAreaCode = ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 3).Value.ToString() != "" ? ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 3).Value.ToString() : "";
                            //                    //    var ExcelDistDescription = ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 5).Value.ToString() != "" ? ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 5).Value.ToString() : "";

                            //                    //    if (ExcelOpCode.IsNullOrEmpty() && ExcelOpDescription.IsNullOrEmpty() &&
                            //                    //       ExcelAreaCode.IsNullOrEmpty() && ExcelDistDescription.IsNullOrEmpty())
                            //                    //    {
                            //                    //        break;
                            //                    //    }
                            //                    //    else if (ExcelAreaCode.IsNullOrEmpty() && ExcelDistDescription.IsNullOrEmpty())
                            //                    //    {
                            //                    //        break;
                            //                    //    }

                            //                    //    if (ExcelOpCode == "" && ExcelOpDescription == "" && ExcelAreaCode == "" && ExcelDistDescription == "")
                            //                    //    {
                            //                    //        break;
                            //                    //    }

                            //                    //    if (ExcelDistDescription != "")
                            //                    //    {
                            //                    //        auxDistribution = ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 5).Value.ToString() != "" ? ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 5).Value.ToString() : "";
                            //                    //    }
                            //                    //    else if (ExcelDistDescription == "")
                            //                    //    {
                            //                    //        ExcelDistDescription = auxDistribution;
                            //                    //    }



                            //                    //    if (ProductExist != null && ProductExist.Similarity > 0.5) // Ajusta este umbral según tus necesidades
                            //                    //    {
                            //                    //        PathResume.ProductID = ProductExist.Product.ProductId;
                            //                    //    }

                            //                    //    // Buscar el ID de planta en el diccionario de plantas
                            //                    //    var planta = PlantsDictionary.Values.FirstOrDefault(p => p.PlantId == plantnameid);
                            //                    //    if (planta != null)
                            //                    //    {
                            //                    //        PathResume.PlantId = planta.PlantId;
                            //                    //    }


                            //                    //    if (PathResume.PlantId > 0)
                            //                    //    {// Buscar coincidencia en area
                            //                    //     //var coincidenciasAreas = AreasDictionary.Where(pair => pair.Key.Item1 == PathResume.PlantId) // Filtramos por ID de planta
                            //                    //     //.Select(pair => new
                            //                    //     //{
                            //                    //     //    Area = pair.Value,
                            //                    //     //    Similarity = (pair.Value.Code == ExcelAreaCode ? 1 : 0)
                            //                    //     //})
                            //                    //     //.OrderByDescending(result => result.Similarity)
                            //                    //     //.FirstOrDefault();


                            //                    //        var coincidenciasAreas = AreasDictionary
                            //                    //                .Where(pair => pair.Key.Item1 == PathResume.PlantId) // Filtramos por ID de planta
                            //                    //                .Select(pair => new
                            //                    //                {
                            //                    //                    Area = pair.Value,
                            //                    //                    Similarity = pair.Value.Code.Equals(ExcelAreaCode)
                            //                    //                        ? 1.0 // Si los códigos coinciden exactamente, la similitud es máxima
                            //                    //                        : 1 - pair.Value.Code.JaccardDistance(ExcelAreaCode)
                            //                    //                })
                            //                    //                .OrderByDescending(result => result.Similarity)
                            //                    //                .FirstOrDefault();


                            //                    //        if (coincidenciasAreas != null && coincidenciasAreas.Similarity >= 0.70) // Ajusta este umbral según tus necesidades
                            //                    //        {
                            //                    //            PathResume.AreaId = coincidenciasAreas.Area.AreaId;
                            //                    //            PathResume.DescripcionArea = coincidenciasAreas.Area.Description;
                            //                    //        }


                            //                    //        if (PathResume.AreaId > 0)
                            //                    //        {
                            //                    //            // Buscar coincidencia en distribucion
                            //                    //            var coincidenciasDistributions = DistributionsDictionary
                            //                    //                .Where(pair => pair.Key.Item1 == PathResume.PlantId && pair.Key.Item2 == PathResume.AreaId)
                            //                    //                .Select(pair => new
                            //                    //                {
                            //                    //                    Distribution = pair.Value,
                            //                    //                    Similarity = 1 - pair.Value.Description.JaccardDistance(ExcelDistDescription)
                            //                    //                })
                            //                    //                .OrderByDescending(result => result.Similarity)
                            //                    //                .FirstOrDefault();

                            //                    //            if (coincidenciasDistributions != null && coincidenciasDistributions.Similarity > 0.5)
                            //                    //            {
                            //                    //                PathResume.DistributionId = coincidenciasDistributions.Distribution.DistributionId;
                            //                    //                PathResume.DescripcionDistribucion = coincidenciasDistributions.Distribution.Description;
                            //                    //            }


                            //                    //            if (PathResume.DistributionId > 0)
                            //                    //            {

                            //                    //                var coincidenciasOperaciones = OperationsDictionary
                            //                    //               .Where(pair => pair.Key.Item1 == PathResume.PlantId && pair.Key.Item2 == PathResume.AreaId && pair.Key.Item3 == PathResume.DistributionId)
                            //                    //               .Select(pair => new
                            //                    //               {
                            //                    //                   Operation = pair.Value,
                            //                    //                   Similarity = (pair.Value.Code == ExcelOpCode && pair.Value.Description == ExcelOpDescription ? 1 : 0)
                            //                    //               })
                            //                    //               .OrderByDescending(result => result.Similarity)
                            //                    //               .FirstOrDefault();

                            //                    //                if (coincidenciasOperaciones != null && coincidenciasOperaciones.Similarity > 0.5)
                            //                    //                {
                            //                    //                    PathResume.OperationId = coincidenciasOperaciones.Operation.OperationId;
                            //                    //                }

                            //                    //                if (PathResume.OperationId > 0)
                            //                    //                {
                            //                    //                    //existe no se hace nada
                            //                    //                }
                            //                    //                else
                            //                    //                {//No existe hay que crearla
                            //                    //                    var operationForCreate = _mapper.Map<OperationForCreationDto>(new OperationForCreationDto() { Code = ExcelOpCode, Description = ExcelOpDescription, IsActive = true });
                            //                    //                    var finalOperation = _mapper.Map<Operation>(operationForCreate);
                            //                    //                    await _supervisorMobilityRepository.AddOperationForDistributionAsync((int)PathResume.AreaId, (int)PathResume.DistributionId, finalOperation);
                            //                    //                    await _supervisorMobilityRepository.SaveChangesAsync();

                            //                    //                    OperationsDictionary.Add(((int)PathResume.PlantId, (int)PathResume.AreaId, (int)PathResume.DistributionId, finalOperation.OperationId), finalOperation);
                            //                    //                    CountCreateOperation++;
                            //                    //                }


                            //                    //            }//end if distribution >0
                            //                    //            else
                            //                    //            {
                            //                    //                //distribution no existe- se crea todo
                            //                    //                string codeGen = ExcelDistDescription;

                            //                    //                SlugHelper slugHelper = new SlugHelper();
                            //                    //                string slug = slugHelper.GenerateSlug(codeGen);

                            //                    //                var distributionForCreate = _mapper.Map<DistributionForCreationDto>(new DistributionForCreationDto() { Code = slug, Description = ExcelDistDescription, IsActive = true });
                            //                    //                var finalDistribution = _mapper.Map<Distribution>(distributionForCreate);
                            //                    //                await _supervisorMobilityRepository.AddDistributionForPlantAsync((int)PathResume.PlantId, (int)PathResume.AreaId, finalDistribution);
                            //                    //                await _supervisorMobilityRepository.SaveChangesAsync();
                            //                    //                PathResume.DistributionId = finalDistribution.DistributionId;
                            //                    //                DistributionsDictionary.Add(((int)PathResume.PlantId, (int)PathResume.AreaId, (int)PathResume.DistributionId), finalDistribution);

                            //                    //                await _supervisorMobilityRepository.AddDistributionForProductAsync((int)PathResume.ProductID, finalDistribution);

                            //                    //                //la operacion no existira

                            //                    //                var operationForCreate = _mapper.Map<OperationForCreationDto>(new OperationForCreationDto() { Code = ExcelOpCode, Description = ExcelOpDescription, IsActive = true });
                            //                    //                var finalOperation = _mapper.Map<Operation>(operationForCreate);
                            //                    //                await _supervisorMobilityRepository.AddOperationForDistributionAsync((int)PathResume.AreaId, (int)PathResume.DistributionId, finalOperation);
                            //                    //                await _supervisorMobilityRepository.SaveChangesAsync();

                            //                    //                OperationsDictionary.Add(((int)PathResume.PlantId, (int)PathResume.AreaId, (int)PathResume.DistributionId, finalOperation.OperationId), finalOperation);
                            //                    //                CountCreateOperation++;
                            //                    //            }
                            //                    //        }//end if area > 0
                            //                    //        else
                            //                    //        {
                            //                    //            //area no existe- se crea todo
                            //                    //            var areaForCreate = _mapper.Map<AreaForCreationDto>(new AreaForCreationDto() { Code = ExcelAreaCode, Description = ExcelAreaCode, IsActive = true });

                            //                    //            var finalArea = _mapper.Map<Area>(areaForCreate);
                            //                    //            finalArea.PlantId = (int)PathResume.PlantId;

                            //                    //            await _supervisorMobilityRepository.AddArea(finalArea);
                            //                    //            await _supervisorMobilityRepository.AddAreaForPlantAsync((int)PathResume.PlantId, finalArea);
                            //                    //            await _supervisorMobilityRepository.SaveChangesAsync();
                            //                    //            PathResume.AreaId = finalArea.AreaId;

                            //                    //            AreasDictionary.Add(((int)PathResume.PlantId, (int)PathResume.AreaId), finalArea);

                            //                    //            ////
                            //                    //            //la distribuccion no existira
                            //                    //            string codeGen = ExcelDistDescription;

                            //                    //            SlugHelper slugHelper = new SlugHelper();
                            //                    //            string slug = slugHelper.GenerateSlug(codeGen);

                            //                    //            var distributionForCreate = _mapper.Map<DistributionForCreationDto>(new DistributionForCreationDto() { Code = slug, Description = ExcelDistDescription, IsActive = true });
                            //                    //            var finalDistribution = _mapper.Map<Distribution>(distributionForCreate);
                            //                    //            await _supervisorMobilityRepository.AddDistributionForPlantAsync((int)PathResume.PlantId, (int)PathResume.AreaId, finalDistribution);
                            //                    //            await _supervisorMobilityRepository.SaveChangesAsync();
                            //                    //            PathResume.DistributionId = finalDistribution.DistributionId;

                            //                    //            DistributionsDictionary.Add(((int)PathResume.PlantId, (int)PathResume.AreaId, (int)PathResume.DistributionId), finalDistribution);
                            //                    //            await _supervisorMobilityRepository.AddDistributionForProductAsync((int)PathResume.ProductID, finalDistribution);

                            //                    //            //la operacion no existira

                            //                    //            var operationForCreate = _mapper.Map<OperationForCreationDto>(new OperationForCreationDto() { Code = ExcelOpCode, Description = ExcelOpDescription, IsActive = true });
                            //                    //            var finalOperation = _mapper.Map<Operation>(operationForCreate);
                            //                    //            await _supervisorMobilityRepository.AddOperationForDistributionAsync((int)PathResume.AreaId, (int)PathResume.DistributionId, finalOperation);
                            //                    //            await _supervisorMobilityRepository.SaveChangesAsync();

                            //                    //            OperationsDictionary.Add(((int)PathResume.PlantId, (int)PathResume.AreaId, (int)PathResume.DistributionId, finalOperation.OperationId), finalOperation);
                            //                    //            CountCreateOperation++;
                            //                    //        }

                            //                    //    }//end if plant >0
                            //                    //     // no hay chance de que la planta no exista


                            //                    //}//else paginas siguientes

                            //                    var AssyChartExist = await _supervisorMobilityRepository.GetAssyChartForJobObservationAsync((int)PathResume.PlantId, (int)PathResume.AreaId, (int)PathResume.DistributionId);

                            //                    if (AssyChartExist is null)
                            //                    {
                            //                        AssyChartForCreation assychartForCreate = new AssyChartForCreation()
                            //                        {
                            //                            PlantId = (int)PathResume.PlantId,
                            //                            AreaId = (int)PathResume.AreaId,
                            //                            DistributionId = (int)PathResume.DistributionId,
                            //                            CreationDate = DateTime.Now,
                            //                            ModificationDate = DateTime.Now,
                            //                            IsActive = true
                            //                        };

                            //                        var resultCreateAssy = await _assyChartService.CreateAssyChartAsync(assychartForCreate);
                            //                        CountCreateAssycchart++;
                            //                        if (resultCreateAssy != null)
                            //                        {
                            //                            //se crea assy chart cout
                            //                            Debug.WriteLine($"Create assychart id {resultCreateAssy.AssyChardId} plantid {(int)PathResume.PlantId} areaid {(int)PathResume.AreaId} distributionid {(int)PathResume.DistributionId} ");
                            //                        }
                            //                    }

                            //                    retries = 0;
                            //                    // Si la operación tiene éxito, puedes salir del bucle
                            //                    break;
                            //                }
                            //                catch (Exception ex)
                            //                {
                            //                    // Maneja la excepción aquí, si es necesario
                            //                    Debug.WriteLine($"I Value:{i}");
                            //                    Debug.WriteLine($"Intento {retries + 1} falló: {ex.Message}");

                            //                    // Incrementa el número de intentos
                            //                    retries++;

                            //                    // Espera el intervalo de tiempo antes de volver a intentarlo
                            //                    await Task.Delay(retryInterval);
                            //                }



                            //            }//end While 
                            //            Debug.WriteLine($"{productCode} :{i}");
                            //            i++;
                            //        }//end is not empety row
                            //    }//end else first roe

                            //}//end foreach de renglones en pagina

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


            }//end scope
        }

        public async Task ProcessPathsAsync(string trustedFileNameForStorage, int UserIdUpload, CancellationToken stoppingToken)
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

                User userEntity = await _supervisorMobilityRepository.GetUserAsync(UserIdUpload, false);


                //GET rutas CDMS
                CDMS_GOS_Directory GOSFolders = new CDMS_GOS_Directory();
                TreeItemData rootNodeGOS = new TreeItemData();

                CDMS_CCP_Directory CCPFolders = new CDMS_CCP_Directory();
                TreeItemData rootNodeCCP = new TreeItemData();

                CDMS_HOE_Directory HOEFolders = new CDMS_HOE_Directory();
                TreeItemData rootNodeHOE = new TreeItemData();

                //  Get Tree data 

                IEnumerable<Plant> Plants = await _supervisorMobilityRepository.GetPlantsAsync();
                IEnumerable<Product> Products = await _supervisorMobilityRepository.GetProductsAsync();

                Dictionary<int, Plant> PlantsDictionary = new Dictionary<int, Plant>();
                Dictionary<(int, int), Area> AreasDictionary = new Dictionary<(int, int), Area>();
                Dictionary<(int, int, int), Distribution> DistributionsDictionary = new Dictionary<(int, int, int), Distribution>();

                foreach (Plant plantElement in Plants)
                {
                    PlantsDictionary.Add(plantElement.PlantId, plantElement);

                    IEnumerable<Area> areasPlant = await _supervisorMobilityRepository.GetAreasForPlantAsync(plantElement.PlantId);

                    foreach (Area areaElement in areasPlant)
                    {
                        AreasDictionary.Add((plantElement.PlantId, areaElement.AreaId), areaElement);

                        IEnumerable<Distribution> distributions = await _supervisorMobilityRepository.GetDistributionsForAreaAsync(areaElement.AreaId);

                        foreach (Distribution distribution in distributions)
                        {
                            DistributionsDictionary.Add((plantElement.PlantId, areaElement.AreaId, distribution.DistributionId), distribution);
                        }
                    }
                }





                try
                {

                    try
                    {
                        try
                        {
                            var response = await _bridgeHttpClient.GetAsync("SMGos/GetDirectoryPathsGos");

                            if (response.IsSuccessStatusCode)
                            {
                                var result = await response.Content.ReadFromJsonAsync<CDMS_GOS_Directory>();
                                GOSFolders = result;
                            }
                            else
                            {
                                //await _js.InvokeVoidAsync("alert", $"Error get folders: {response.Content.ReadAsStringAsync().Result}");
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

                    if (GOSFolders != null)
                    {
                        rootNodeGOS = _treeService.ConstruirArbolGOS(GOSFolders.operation);
                    }

                    try
                    {
                        try
                        {
                            var response = await _bridgeHttpClient.GetAsync("SMCcp/GetDirectoryPathsCcp");

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

                    if (CCPFolders != null)
                    {
                        rootNodeCCP = _treeService.ConstruirArbolCCP(CCPFolders.operation);
                    }

                    try
                    {
                        try
                        {
                            var response = await _bridgeHttpClient.GetAsync("SMHoe/GetDirectoryPaths");

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
                    if (HOEFolders != null)
                    {
                        rootNodeHOE = _treeService.ConstruirArbolHOE(HOEFolders.operation);
                    }

                }
                catch (Exception ex)
                {

                }

                //Start Massive Upload 
                string filepath = Directory.GetCurrentDirectory().ToString() + "\\uploads\\massive\\" + trustedFileNameForStorage;
                try
                {
                    using (var workBook = new XLWorkbook(filepath))
                    {
                        var pages = workBook.Worksheets.Count;
                        for (int p = 2; p <= pages; p++)
                        {

                            IXLWorksheet ws = workBook.Worksheet(p);

                            var productCode = ws.Name;
                            Debug.WriteLine($"Product Name: {productCode}");

                            var ProductExist = Products.Select(pair => new
                            {
                                Product = pair,
                                Similarity = 1 - pair.Code.JaccardDistance(productCode)
                            }).OrderByDescending(result => result.Similarity).FirstOrDefault();


                            string HoeAuxPath = "";
                            string DistributionAux = "";

                            bool firstRow = true;
                            int i = 1;
                            foreach (IXLRow row in ws.Rows())
                            {


                                if (firstRow)
                                {
                                    firstRow = false;
                                }
                                else
                                {
                                    if (!row.IsEmpty())
                                    {


                                        int maxRetries = 5; // Número máximo de intentos
                                        TimeSpan retryInterval = TimeSpan.FromSeconds(5); // Intervalo de tiempo entre intentos (5 segundos en este caso)
                                        int retries = 0;

                                        PathInfo PathResume = new PathInfo();
                                        TreeItemData? mejorCoincidenciaHOE = null;
                                        TreeItemData? mejorCoincidenciaGOS = null;
                                        TreeItemData? mejorCoincidenciaCCP = null;


                                        var InfoCodePath = ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 1).Value.ToString() != "" ? ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 1).Value.ToString() : "";
                                        var InfoArea = ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 3).Value.ToString() != "" ? ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 3).Value.ToString() : "";
                                        var InfoDistribution = ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 5).Value.ToString() != "" ? ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 5).Value.ToString() : "";
                                        var InfoHOEPath = ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 6).Value.ToString() != "" ? ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 6).Value.ToString() : "";
                                        var InfoGOSPath = ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 7).Value.ToString() != "" ? ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 7).Value.ToString() : "";
                                        var InfoCCPPath = ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 8).Value.ToString() != "" ? ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 8).Value.ToString() : "";
                                        var InfoCDPath = ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 9).Value.ToString() != "" ? ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 9).Value.ToString() : "";

                                        if (ProductExist != null && ProductExist.Similarity > 0.5) // Ajusta este umbral según tus necesidades
                                        {
                                            PathResume.ProductID = ProductExist.Product.ProductId;
                                        }

                                        if (string.IsNullOrWhiteSpace(InfoCodePath) && string.IsNullOrWhiteSpace(InfoArea) && string.IsNullOrWhiteSpace(InfoDistribution) && string.IsNullOrWhiteSpace(InfoHOEPath) && string.IsNullOrWhiteSpace(InfoGOSPath) && string.IsNullOrWhiteSpace(InfoCCPPath))
                                        {
                                            i++;
                                            continue;
                                        }


                                        if (!string.IsNullOrWhiteSpace(InfoDistribution))
                                        {
                                            DistributionAux = ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 5).Value.ToString() != "" ? ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 5).Value.ToString() : "";
                                        }
                                        else if (string.IsNullOrWhiteSpace(InfoDistribution))
                                        {
                                            InfoDistribution = DistributionAux;
                                        }


                                        if (!string.IsNullOrWhiteSpace(InfoHOEPath))
                                        {
                                            HoeAuxPath = ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 6).Value.ToString() != "" ? ws.Cell(row.RangeAddress.FirstAddress.RowNumber, 6).Value.ToString() : "";

                                            string HOESinSaltosDeLinea = InfoHOEPath.Replace("\n", "");
                                            string rutaHOENormalizada = _treeService.NormalizarRutaUsuario(HOESinSaltosDeLinea);

                                            mejorCoincidenciaHOE = _treeService.EncontrarMejorCoincidenciaDifusa(rootNodeHOE, rutaHOENormalizada, productCode);
                                        }
                                        else if (string.IsNullOrWhiteSpace(InfoHOEPath) && !string.IsNullOrWhiteSpace(HoeAuxPath))
                                        {

                                            var splitHoe = HoeAuxPath.Split(".");

                                            var conjunto1 = new HashSet<char>(splitHoe.Last());
                                            var conjunto2 = new HashSet<char>(InfoDistribution);

                                            int interseccion = 0;
                                            int union = conjunto1.Count + conjunto2.Count;

                                            foreach (var caracter in conjunto1)
                                            {
                                                if (conjunto2.Contains(caracter))
                                                    interseccion++;
                                            }

                                            var result = (double)interseccion / (double)(union - interseccion);

                                            if (result > 0.9)
                                            {
                                                InfoHOEPath = HoeAuxPath;
                                                string HOESinSaltosDeLinea = InfoHOEPath.Replace("\n", "");
                                                string rutaHOENormalizada = _treeService.NormalizarRutaUsuario(HOESinSaltosDeLinea);

                                                mejorCoincidenciaHOE = _treeService.EncontrarMejorCoincidenciaDifusa(rootNodeHOE, rutaHOENormalizada, productCode);
                                            }
                                            else
                                            {
                                                HoeAuxPath = "";
                                            }

                                        }


                                        if (mejorCoincidenciaHOE != null)
                                        {
                                            PathResume.HOE = mejorCoincidenciaHOE.Ruta;
                                            Debug.WriteLine("Mejor coincidencia: " + mejorCoincidenciaHOE.Ruta);

                                            // Buscar coincidencias en Plantas

                                            string[] segmentos = mejorCoincidenciaHOE.Ruta.Split('/');
                                            if (segmentos.Length > 0)
                                            {
                                                string codigoPlanta = segmentos.FirstOrDefault();

                                                // Buscar el ID de planta en el diccionario de plantas
                                                var planta = PlantsDictionary.Values.FirstOrDefault(p => codigoPlanta.Contains(p.Code));
                                                if (planta != null)
                                                {
                                                    PathResume.PlantId = planta.PlantId;
                                                }
                                            }

                                            if (PathResume.PlantId > 0)
                                            {// Buscar coincidencia en area


                                                var coincidenciasAreas = AreasDictionary
                                                               .Where(pair => pair.Key.Item1 == PathResume.PlantId) // Filtramos por ID de planta
                                                               .Select(pair => new
                                                               {
                                                                   Area = pair.Value,
                                                                   Similarity = pair.Value.Code.Equals(InfoArea)
                                                                       ? 1.0 // Si los códigos coinciden exactamente, la similitud es máxima
                                                                       : 1 - pair.Value.Code.JaccardDistance(InfoArea)
                                                               })
                                                               .OrderByDescending(result => result.Similarity)
                                                               .FirstOrDefault();


                                                if (coincidenciasAreas != null && coincidenciasAreas.Similarity > 0.5) // Ajusta este umbral según tus necesidades
                                                {
                                                    PathResume.AreaId = coincidenciasAreas.Area.AreaId;
                                                    PathResume.DescripcionArea = coincidenciasAreas.Area.Description;
                                                }


                                                if (PathResume.AreaId > 0)
                                                {
                                                    // Buscar coincidencia en distribucion
                                                    var coincidenciasDistributions = DistributionsDictionary
                                                        .Where(pair => pair.Key.Item1 == PathResume.PlantId && pair.Key.Item2 == PathResume.AreaId)
                                                        .Select(pair => new
                                                        {
                                                            Distribution = pair.Value,
                                                            Similarity = 1 - pair.Value.Description.JaccardDistance(InfoDistribution)
                                                        })
                                                        .OrderByDescending(result => result.Similarity)
                                                        .FirstOrDefault();

                                                    if (coincidenciasDistributions != null && coincidenciasDistributions.Similarity > 0.5) // Ajusta este umbral según tus necesidades
                                                    {
                                                        PathResume.DistributionId = coincidenciasDistributions.Distribution.DistributionId;
                                                        PathResume.DescripcionDistribucion = coincidenciasDistributions.Distribution.Description;
                                                    }


                                                    if (PathResume.DistributionId > 0)
                                                    {

                                                        Debug.WriteLine($"CODE PATH RESUME DESDE HOE");
                                                        Debug.WriteLine($"PLANT : {PathResume.PlantId}");
                                                        Debug.WriteLine($"AREA : {PathResume.AreaId}");
                                                        Debug.WriteLine($"DISTRIBUTION : {PathResume.DistributionId}");


                                                    }//end if distribution >0
                                                }//end if area > 0

                                            }//end if plant >0


                                        }
                                        else
                                        {
                                            Debug.WriteLine("No se encontró ninguna coincidencia HOE.");
                                        }



                                        if (InfoGOSPath.Contains(">") && InfoGOSPath != "")
                                        {
                                            string GOSSinSaltoDeLinea = InfoGOSPath.Replace("\n", "");

                                            GOSSinSaltoDeLinea = InfoGOSPath.Replace("GOS/HO/HP >", "");

                                            string rutaGOSNormalizada = _treeService.NormalizarRutaUsuario(GOSSinSaltoDeLinea);

                                            mejorCoincidenciaGOS = _treeService.EncontrarMejorCoincidenciaDifusaInternal(rootNodeGOS, rutaGOSNormalizada, productCode);


                                        }

                                        if (mejorCoincidenciaGOS != null && mejorCoincidenciaHOE == null)
                                        {
                                            Debug.WriteLine("HOE IS NULL ");
                                            Debug.WriteLine("Mejor coincidencia GOS: " + mejorCoincidenciaGOS.Ruta);


                                            // Buscar coincidencias en Plantas

                                            string[] segmentos = mejorCoincidenciaGOS.Ruta.Split('/');
                                            if (segmentos.Length > 0)
                                            {
                                                string codigoPlanta = segmentos.FirstOrDefault();

                                                // Buscar el ID de planta en el diccionario de plantas
                                                var planta = PlantsDictionary.Values.FirstOrDefault(p => codigoPlanta.Contains(p.Code));
                                                if (planta != null)
                                                {
                                                    PathResume.PlantId = planta.PlantId;
                                                }
                                            }

                                            if (PathResume.PlantId > 0)
                                            {// Buscar coincidencia en area
                                                var coincidenciasAreas = AreasDictionary.Where(pair => pair.Key.Item1 == PathResume.PlantId) // Filtramos por ID de planta
                                                .Select(pair => new
                                                {
                                                    Area = pair.Value,
                                                    Similarity = 1 - pair.Value.Code.JaccardDistance(InfoArea)
                                                })
                                                .OrderByDescending(result => result.Similarity)
                                                .FirstOrDefault();

                                                if (coincidenciasAreas != null && coincidenciasAreas.Similarity > 0.5) // Ajusta este umbral según tus necesidades
                                                {
                                                    PathResume.AreaId = coincidenciasAreas.Area.AreaId;
                                                    PathResume.DescripcionArea = coincidenciasAreas.Area.Description;
                                                }


                                                if (PathResume.AreaId > 0)
                                                {
                                                    // Buscar coincidencia en distribucion
                                                    var coincidenciasDistributions = DistributionsDictionary
                                                        .Where(pair => pair.Key.Item1 == PathResume.PlantId && pair.Key.Item2 == PathResume.AreaId)
                                                        .Select(pair => new
                                                        {
                                                            Distribution = pair.Value,
                                                            Similarity = 1 - pair.Value.Description.JaccardDistance(InfoDistribution)
                                                        })
                                                        .OrderByDescending(result => result.Similarity)
                                                        .FirstOrDefault();

                                                    if (coincidenciasDistributions != null && coincidenciasDistributions.Similarity > 0.5) // Ajusta este umbral según tus necesidades
                                                    {
                                                        PathResume.DistributionId = coincidenciasDistributions.Distribution.DistributionId;
                                                        PathResume.DescripcionDistribucion = coincidenciasDistributions.Distribution.Description;
                                                    }


                                                    if (PathResume.DistributionId > 0)
                                                    {

                                                        Debug.WriteLine($"CODE PATH RESUME GOS");
                                                        Debug.WriteLine($"PLANT : {PathResume.PlantId}");
                                                        Debug.WriteLine($"AREA : {PathResume.AreaId}");
                                                        Debug.WriteLine($"DISTRIBUTION : {PathResume.DistributionId}");


                                                    }//end if distribution >0
                                                }//end if area > 0

                                            }//end if plant >0
                                        }




                                        if (InfoCCPPath.Contains(">") && InfoCCPPath != "")
                                        {

                                            string CCPSinSaltoDeLinea = InfoCCPPath.Replace("\n", "");

                                            CCPSinSaltoDeLinea = CCPSinSaltoDeLinea.Replace("CCP >", "");

                                            string rutaCCPNormalizada = _treeService.NormalizarRutaUsuario(CCPSinSaltoDeLinea);

                                            mejorCoincidenciaCCP = _treeService.EncontrarMejorCoincidenciaDifusaInternal(rootNodeCCP, rutaCCPNormalizada, productCode);

                                        }


                                        if (mejorCoincidenciaHOE == null && mejorCoincidenciaGOS == null && mejorCoincidenciaCCP != null)
                                        {
                                            Debug.WriteLine("HOE & GOS IS NULL");
                                            Debug.WriteLine("Mejor coincidencia CCP: " + mejorCoincidenciaCCP.Ruta);
                                            string[] segmentos = mejorCoincidenciaCCP.Ruta.Split('/');
                                            if (segmentos.Length > 0)
                                            {
                                                string codigoPlanta = segmentos.FirstOrDefault();

                                                // Buscar el ID de planta en el diccionario de plantas
                                                var planta = PlantsDictionary.Values.FirstOrDefault(p => codigoPlanta.Contains(p.Code));
                                                if (planta != null)
                                                {
                                                    PathResume.PlantId = planta.PlantId;
                                                }
                                            }

                                            if (PathResume.PlantId > 0)
                                            {// Buscar coincidencia en area
                                                var coincidenciasAreas = AreasDictionary.Where(pair => pair.Key.Item1 == PathResume.PlantId) // Filtramos por ID de planta
                                                .Select(pair => new
                                                {
                                                    Area = pair.Value,
                                                    Similarity = 1 - pair.Value.Code.JaccardDistance(InfoArea)
                                                })
                                                .OrderByDescending(result => result.Similarity)
                                                .FirstOrDefault();

                                                if (coincidenciasAreas != null && coincidenciasAreas.Similarity > 0.5) // Ajusta este umbral según tus necesidades
                                                {
                                                    PathResume.AreaId = coincidenciasAreas.Area.AreaId;
                                                    PathResume.DescripcionArea = coincidenciasAreas.Area.Description;
                                                }


                                                if (PathResume.AreaId > 0)
                                                {
                                                    // Buscar coincidencia en distribucion
                                                    var coincidenciasDistributions = DistributionsDictionary
                                                        .Where(pair => pair.Key.Item1 == PathResume.PlantId && pair.Key.Item2 == PathResume.AreaId)
                                                        .Select(pair => new
                                                        {
                                                            Distribution = pair.Value,
                                                            Similarity = 1 - pair.Value.Description.JaccardDistance(InfoDistribution)
                                                        })
                                                        .OrderByDescending(result => result.Similarity)
                                                        .FirstOrDefault();

                                                    if (coincidenciasDistributions != null && coincidenciasDistributions.Similarity > 0.5) // Ajusta este umbral según tus necesidades
                                                    {
                                                        PathResume.DistributionId = coincidenciasDistributions.Distribution.DistributionId;
                                                        PathResume.DescripcionDistribucion = coincidenciasDistributions.Distribution.Description;
                                                    }


                                                    if (PathResume.DistributionId > 0)
                                                    {

                                                        Debug.WriteLine($"CODE PATH RESUME CCP");
                                                        Debug.WriteLine($"PLANT : {PathResume.PlantId}");
                                                        Debug.WriteLine($"AREA : {PathResume.AreaId}");
                                                        Debug.WriteLine($"DISTRIBUTION : {PathResume.DistributionId}");


                                                    }//end if distribution >0
                                                }//end if area > 0

                                            }//end if plant >0
                                        }
                                        else if (mejorCoincidenciaHOE == null && mejorCoincidenciaGOS == null && mejorCoincidenciaCCP == null)
                                        {
                                            Debug.WriteLine("HOE & GOS & CCP IS NULL");
                                        }




                                        if ((int)PathResume.PlantId != null && (int)PathResume.AreaId != null && (int)PathResume.DistributionId != null)
                                            while (retries < maxRetries)
                                            {
                                                try
                                                {
                                                    //Search assychart Exist
                                                    var AssyChartExist = await _supervisorMobilityRepository.GetAssyChartForJobObservationAsync((int)PathResume.PlantId, (int)PathResume.AreaId, (int)PathResume.DistributionId);


                                                    if (AssyChartExist != null)
                                                    {
                                                        var CodePathExist = await _supervisorMobilityRepository.TryFindCodePathItemAsync(AssyChartExist.AssyChardId, InfoCodePath);

                                                        if (CodePathExist == null)
                                                        {

                                                            //procedimiento de path
                                                            SOSCodePath CodePath = new SOSCodePath();

                                                            CodePath.Code = InfoCodePath;
                                                            //TreeItemData? mejorCoincidenciaHOE = null;
                                                            //TreeItemData? mejorCoincidenciaGOS = null;
                                                            //TreeItemData? mejorCoincidenciaCCP = null;

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

                                                            //Añadir el common direction

                                                            //Añadimso distribucion y Producto

                                                            CodePath.DistributionId = (int)PathResume.DistributionId;
                                                            CodePath.ProductId = PathResume.ProductID;




                                                            //creamos assychart si no existe
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

                                                                if (resultCreateAssy != null)
                                                                {
                                                                    //se crea assy chart cout
                                                                    Debug.WriteLine($"Create assychart id {resultCreateAssy.AssyChardId} plantid {(int)PathResume.PlantId} areaid {(int)PathResume.AreaId} distributionid {(int)PathResume.DistributionId} ");


                                                                    //Completamos codePath
                                                                    CodePath.AssyChardId = AssyChartExist.AssyChardId;

                                                                    //Crear Code Path
                                                                    await _supervisorMobilityRepository.AssychartCreateCodePath(CodePath);

                                                                    //aqui se añade el path creado

                                                                    _supervisorMobilityRepository.AssychartAddCodePath(resultCreateAssy, CodePath);
                                                                    await _supervisorMobilityRepository.SaveChangesAsync();
                                                                }

                                                            }
                                                            else
                                                            {
                                                                //Completamos codePath
                                                                CodePath.AssyChardId = AssyChartExist.AssyChardId;

                                                                //Crear Code Path
                                                                await _supervisorMobilityRepository.AssychartCreateCodePath(CodePath);

                                                                //aqui añadimos el path a assy chart
                                                                _supervisorMobilityRepository.AssychartAddCodePath(AssyChartExist, CodePath);
                                                                await _supervisorMobilityRepository.SaveChangesAsync();
                                                            }
                                                        }

                                                    }


                                                    retries = 0;

                                                    Debug.WriteLine($"Intento {retries} Linea Position [{i}]");

                                                    // Si la operación tiene éxito, puedes salir del bucle
                                                    break;
                                                }
                                                catch (Exception ex)
                                                {
                                                    // Maneja la excepción aquí, si es necesario
                                                    Console.WriteLine($"Intento {retries + 1} Linea Position [{i}] falló: {ex.Message}");

                                                    // Incrementa el número de intentos
                                                    retries++;

                                                    // Espera el intervalo de tiempo antes de volver a intentarlo
                                                    await Task.Delay(retryInterval);
                                                }



                                            }//While

                                    }//end is not empety row
                                }//end else first roe
                                i++;
                            }//end foreach

                        }//for de paginas

                    }//end using



                }//end try
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error Path: {ex.Message.ToString()}");
                }//end trycatch to add excel to list



                int maxRetriesMail = 5; // Número máximo de intentos
                TimeSpan retryIntervalMail = TimeSpan.FromSeconds(5); // Intervalo de tiempo entre intentos (5 segundos en este caso)
                int retriesMail = 0;

                while (retriesMail < maxRetriesMail)
                {
                    try
                    {
                        //var emailMessage = _email.CreateEmailMessage(userEntity.Email, "Paths document has been processed, you can now review its contents on the Paths details page.");
                        //_email.Send(emailMessage);
                        Debug.WriteLine($"Correo Enviado Paths");

                        break;
                    }
                    catch (Exception ex)
                    {

                        // Maneja la excepción aquí, si es necesario
                        Debug.WriteLine($"Fallo crear Succes Notification: {ex.Message}");

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
                        NotyFinish.NotificationType = $"Paths Procces {DateTime.Now}";
                        NotyFinish.NotificationText = $"Paths document has been processed, you can now review its contents on the Paths details page.";

                        NotyFinish.MadeBy = "Paths Process System ";
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
                            NotyError.NotificationType = $"Paths Procces Finish: {DateTime.Now}";
                            NotyError.NotificationText = $"Paths procces document";

                            NotyError.MadeBy = "Paths System";
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

            }//end using scope
        }

    }
}
