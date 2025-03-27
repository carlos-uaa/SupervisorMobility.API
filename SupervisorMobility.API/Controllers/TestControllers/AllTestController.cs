using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.TreeStruct;
using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.DataAccess.Services.TreeServices;
using SupervisorMobility.API.Entities.CDMS;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.FileUploadDto;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using SupervisorMobility.API.Services;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using SupervisorMobility.API.Models.Users;

namespace SupervisorMobility.API.Controllers.SOS_Controllers
{
    [Route("api/AllTest")]
    [ApiController]
    public class AllTestController : Controller
    {
        private readonly ISOS_ProcessRepository _ProcessRepository;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;
        private readonly ISupervisorMobilityRepository _supervisorMobilityRepository;
        private readonly IWebHostEnvironment _env;
        public AllTestController(ISupervisorMobilityRepository supervisorMobilityRepository, IWebHostEnvironment env, IMapper mapper, ISOS_ProcessRepository repository, IServiceProvider serviceProvider)
        {
            _supervisorMobilityRepository = supervisorMobilityRepository ??
                throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _ProcessRepository = repository;
            _mapper = mapper ??
                  throw new ArgumentNullException(nameof(mapper));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        //set rr door lh

        [HttpPost("TestNextYearJob")]
        public async Task<ActionResult> TestNextYearJob(int jobObservationId)
        {
            var jobObservationEntity = await _supervisorMobilityRepository.GetJobObservationAsync(jobObservationId, includeOperations: true);
            if (jobObservationEntity == null) return NotFound("JobObservation no encontrada.");


            jobObservationEntity.PlannedStartDate = jobObservationEntity.StartDate;
            jobObservationEntity.PlannedEndDate = jobObservationEntity.StartDate;
            jobObservationEntity.FinishedDate = jobObservationEntity.StartDate;
            jobObservationEntity.Status = 6;


            List<JobObservation>? nextYearJobs = await _supervisorMobilityRepository.FindNextYearJobObservations(
                (int)jobObservationEntity.PlantId,
                (int)jobObservationEntity.AreaId,
                (int)jobObservationEntity.DistributionId,
                jobObservationEntity.Operations,
                (int)jobObservationEntity.SupervisorId,
                jobObservationEntity.FinishedDate.Value.Year + 1);

            IEnumerable<JobCategoryStructure> _checklistCategories = await _supervisorMobilityRepository.GetChecklistCategoriesAsync(false);
            string jobCategoryStructureIds = "";
            foreach (var category in _checklistCategories)
            {
                jobCategoryStructureIds += category.JobCategoryStructureId + "|";
            }

            if (nextYearJobs == null || nextYearJobs.Count == 0)
            {

                //no existe hay que crearla
                JobObservation newYearJob = new JobObservation();

                newYearJob.Type = 5;

                newYearJob.PlantId = jobObservationEntity.PlantId;
                newYearJob.AreaId = jobObservationEntity.AreaId;
                newYearJob.DistributionId = jobObservationEntity.DistributionId;

                //newYearJob.OperationId = jobObservationEntity.OperationId;
                newYearJob.Operations = jobObservationEntity.Operations;

                newYearJob.SupervisorId = jobObservationEntity.SupervisorId;

                newYearJob.StartDate = jobObservationEntity.FinishedDate.Value.AddYears(1);
                newYearJob.PlannedStartDate = jobObservationEntity.FinishedDate.Value.AddYears(1);
                newYearJob.EndDate = newYearJob.PlannedStartDate;

                newYearJob.SectionIds = jobCategoryStructureIds;


                var res = await _supervisorMobilityRepository.AddJobObservation(newYearJob);

                if (res > 0)
                {
                    Distribution distribution = await _supervisorMobilityRepository.GetDistributionOnlyIdAsync((int)jobObservationEntity.DistributionId, false);

                    //NotificationToCreateDto notifynextYear = new NotificationToCreateDto();
                    //notifynextYear.MadeBy = auser;
                    //notifynextYear.UserId = jobObservationForUpdate.SupervisorId;
                    //notifynextYear.IsAccepted = true;
                    //notifynextYear.IsActive = true;
                    //notifynextYear.NotificationType = $"SOS Anual - New Job Observation";
                    //notifynextYear.NotificationText = "Estimado Supervisor,\\n\\nHemos detectado una Job Observation." +
                    //    " A continuación, te informamos sobre las acciones que se tomarán en función del estado del SOS Anual para el próximo año " +
                    //    "El sistema procederá a crear una nueva entrada en el SOS Anual con la información proporcionada por la Job Observation.:" +
                    //    $"\\n\\n Distribucion: {distribution?.Description} - {distribution?.Code}" +
                    //     $"\\n Fecha {newYearJob.StartDate}" +
                    //    "\\n\\nPor favor, asegúrate de que la información esté actualizada para evitar posibles inconsistencias.\r\n\r\nSaludos cordiales,\r\n[SupervisorMobility]";


                    //var notynextYear = await _assyChartService.CreateNotificationAsync(notifynextYear);
                    //await _supervisorMobilityRepository.SaveChangesAsync();

                }
            }
            else
            {
                // Crear una nueva JobObservation que consolidará las operaciones deseadas

                var consolidatedFutureJob = new JobObservation
                {
                    Type = 5,
                    PlantId = jobObservationEntity.PlantId,
                    AreaId = jobObservationEntity.AreaId,
                    DistributionId = jobObservationEntity.DistributionId,
                    Operations = new List<Operation>(),

                    SupervisorId = jobObservationEntity.SupervisorId,

                    StartDate = jobObservationEntity.FinishedDate.Value.AddYears(1),
                    PlannedStartDate = jobObservationEntity.FinishedDate.Value.AddYears(1),
                    EndDate = jobObservationEntity.FinishedDate.Value.AddYears(1),
                    SectionIds = jobCategoryStructureIds

                };


                // Iterar sobre las jobs futuras y consolidar las operaciones necesarias
                foreach (var futureJob in nextYearJobs)
                {
                    // Iterar sobre cada operación de la futureJob
                    foreach (var op in futureJob.Operations.ToList()) // `ToList` evita la modificación de la colección mientras iteramos
                    {
                        // Si la operación existe en `jobObservationEntity`, la movemos a `consolidatedFutureJob`
                        if (jobObservationEntity.Operations.Any(currentOp => currentOp.OperationId == op.OperationId))
                        {
                            Operation opAdd = await _supervisorMobilityRepository.GetOperationForDistributionAsync((int)jobObservationEntity.DistributionId, (int)op.OperationId);
                            consolidatedFutureJob.Operations.Add(opAdd);

                            futureJob.Operations.Remove(op); // Remover de la futureJob después de consolidarla
                        }
                    }

                    // Eliminar futureJob si se queda sin operaciones
                    if (!futureJob.Operations.Any())
                    {
                        _supervisorMobilityRepository.PermanentDeleteJobObservation(futureJob);
                        //_context.JobObservations.Remove(futureJob);
                    }
                }


                // Validar que `consolidatedFutureJob` contiene todas las operaciones de `jobObservationEntity`
                foreach (var op in jobObservationEntity.Operations)
                {
                    if (!consolidatedFutureJob.Operations.Any(existingOp => existingOp.OperationId == op.OperationId))
                    {
                        consolidatedFutureJob.Operations.Add(op); // Agregar la operación faltante si no está en consolidatedFutureJob
                    }
                }


                var res = await _supervisorMobilityRepository.AddJobObservation(consolidatedFutureJob);

                if (res > 0)
                {

                    //    Distribution distribution = await _supervisorMobilityRepository.GetDistributionOnlyIdAsync((int)jobObservationEntity.DistributionId, false);

                    //    DateTime FechaActual = jobObservationForUpdate.FinishedDate.Value.AddYears(1);


                    //    NotificationToCreateDto NotifyUpdateNextYear = new NotificationToCreateDto();
                    //    NotifyUpdateNextYear.MadeBy = auser;
                    //    NotifyUpdateNextYear.UserId = jobObservationForUpdate.SupervisorId;
                    //    NotifyUpdateNextYear.IsAccepted = true;
                    //    NotifyUpdateNextYear.IsActive = true;
                    //    NotifyUpdateNextYear.NotificationType = $"Actualizacion del SOS Anual - Update Job Observation";
                    //    NotifyUpdateNextYear.NotificationText = $"Estimado Supervisor,\\n\\nHemos detectado una Job Observation." +
                    //        " A continuación, te informamos sobre las acciones que se tomarán en función del estado del SOS Anual para el próximo año: " +
                    //        $"\\n Distribucion: {distribution?.Description} - {distribution?.Code}" +
                    //        $"\\n{NextYearJob.StartDate} → {FechaActual}" +
                    //        "\\n\\nLos datos relacionados serán actualizados automáticamente con la nueva información." +
                    //        "\\n\\nPor favor, asegúrate de que la información esté actualizada para evitar posibles inconsistencias.\r\n\r\nSaludos cordiales,\r\n[SupervisorMobility]";

                    //    var notynextYearUpdate = await _assyChartService.CreateNotificationAsync(NotifyUpdateNextYear);
                    //    NextYearJob.StartDate = FechaActual;

                    //    await _supervisorMobilityRepository.SaveChangesAsync();
                }


            }



            return Ok();
        }

        [HttpPost("TestFindPath")]
        public async Task<ActionResult> Test(int? plantid, int? areaid, int? distributionid, int? productid)
        {
            var itemToReturn = new object();
            int p_id = 4;
            int a_id = 36;
            int d_id = 1436;
            int m_id = 3;

            if (plantid != null && areaid != null && distributionid != null && productid != null)
            {
                p_id = (int)plantid;
                a_id = (int)areaid;
                d_id = (int)distributionid;
                m_id = (int)productid;
            }
            TreeItemData? nodoHoe = null;
            TreeItemData? nodoGos = null;
            TreeItemData? nodoCcp = null;

            using (var scope = _serviceProvider.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;

                var customHttp = serviceProvider.GetRequiredService<CustomHttpClientService>();
                var _bridgeHttpClient = customHttp.GetBridgeHttpClient();
                var _context = serviceProvider.GetRequiredService<SupervisorMobilityContext>();


                var _treeService = serviceProvider.GetRequiredService<ITreeService>();
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


                var coincidenciasplanta = PlantsDictionary.Values.FirstOrDefault(p => p.PlantId == p_id);

                var coincidenciasAreas = AreasDictionary.Where(pair => pair.Key.Item1 == p_id && pair.Key.Item2 == a_id).FirstOrDefault();

                var coincidenciasDistributions = DistributionsDictionary.Where(pair => pair.Key.Item1 == p_id && pair.Key.Item2 == a_id && pair.Key.Item3 == d_id).FirstOrDefault();

                var coincidenciaProduct = Products.Where(p => p.ProductId == m_id).FirstOrDefault();

                nodoHoe = _treeService.EncontrarNodoMejorCoincidencia(rootNodeHOE, coincidenciasplanta, "produccion", coincidenciasAreas.Value, coincidenciasDistributions.Value, coincidenciaProduct);
                if (nodoHoe != null)
                {
                    Console.WriteLine($"Nodo [HOE] c: {nodoHoe?.Ruta}");
                    Debug.WriteLine($"Nodo [HOE]: {nodoHoe?.Ruta}");
                }
                else
                {
                    Console.WriteLine($"[HOE] No encontrado :c  ");
                    Debug.WriteLine($"HOE No encontrado :c  ");


                }

                nodoGos = _treeService.EncontrarNodoMejorCoincidencia(rootNodeGOS, coincidenciasplanta, "produccion", coincidenciasAreas.Value, coincidenciasDistributions.Value, coincidenciaProduct);
                if (nodoGos != null)
                {
                    Console.WriteLine($"Nodo [GOS] c: {nodoGos?.Ruta}");
                    Debug.WriteLine($"Nodo [GOS]: {nodoGos?.Ruta}");
                }
                else
                {
                    Console.WriteLine($"[GOS] No encontrado :c  ");
                    Debug.WriteLine($"GOS No encontrado :c  ");
                }

                nodoCcp = _treeService.EncontrarNodoMejorCoincidencia(rootNodeCCP, coincidenciasplanta, "produccion", coincidenciasAreas.Value, coincidenciasDistributions.Value, coincidenciaProduct);
                if (nodoCcp != null)
                {
                    Console.WriteLine($"Nodo [ccp] c: {nodoCcp?.Ruta}");
                    Debug.WriteLine($"Nodo [ccp]: {nodoCcp?.Ruta}");
                }
                else
                {
                    Console.WriteLine($"[ccp] No encontrado :c  ");
                    Debug.WriteLine($"ccp No encontrado :c  ");
                }

                itemToReturn = new
                {
                    Hoe = $"Nodo [HOE] : {nodoHoe?.Ruta}",
                    Gos = $"Nodo [GOS] : {nodoGos?.Ruta}",
                    Ccp = $"Nodo [CCP] : {nodoCcp?.Ruta}",
                    PlantaCode = coincidenciasplanta.Description.ToString(),
                    Planta = coincidenciasplanta.Code.ToString(),
                    AreaCode = coincidenciasAreas.Value.Code,
                    Area = coincidenciasAreas.Value.Description,
                    DistributionCode = coincidenciasDistributions.Value.Code,
                    Distribution = coincidenciasDistributions.Value.Description,
                    ProductCode = coincidenciaProduct.Code,
                    Product = coincidenciaProduct.Description,
                };
            }


            return Ok(itemToReturn);

        }

        [HttpPost("VideoTestUpload")]
        public async Task<ActionResult> VideoTestUpload([FromForm] ManyVideosDto request)
        {
            int statusCode = 200;
            int failed = 0;
            string message = "Ok";
            if (request.Files != null)
            {
                string basePath = System.IO.Path.Combine(_env.ContentRootPath, "uploads\\TestUploads");
                try
                {
                    if (!Directory.Exists(basePath))
                        Directory.CreateDirectory(basePath);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex);
                    // handle them here
                }

                foreach (var file in request.Files)
                {
                    try
                    {
                        #region local
                        string filePath = System.IO.Path.Combine(basePath, file.FileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        statusCode = 400;
                        failed++;
                    }
                }
                if (failed == request.Files.Count) message = "all files failed to upload";
                else if (failed > 1 && failed < request.Files.Count) message = "some files failed to upload";
            }
            return StatusCode(statusCode, message);
        }

        [HttpPost("MassiveUpdateUserPayroll")]
        public async Task<ActionResult> MassiveUpdateUserPayroll(IFormFile file)
        {
            //Este controlador sirve para realizar una actualizacion masiva para aquellos usurios logren encontrarse
            //dentro del sistema y no tengan un numero de nomina asignado, ya sea por que no se guardo en su momento 
            //no contaba con uno por el tipo de usuario. Cambios afectados en Febrero-Marzo 2025
            // Indecisiones COMPAS. . . 

            var uploadResult = new FileUploadForCreationDto();
            string trustedFileNameForStorage = string.Empty;
            var unstrustedFileName = file.FileName;

            trustedFileNameForStorage = Path.GetRandomFileName();
            trustedFileNameForStorage = System.IO.Path.ChangeExtension(trustedFileNameForStorage, ".xlsx");
            var path = Path.Combine(_env.ContentRootPath, "uploads\\massive", trustedFileNameForStorage);
            var directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            await using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                // Utiliza "await" para asegurarte de que se complete la copia del archivo antes de continuar
                await file.CopyToAsync(fs);
            }


            string filepath = Directory.GetCurrentDirectory().ToString() + "\\uploads\\massive\\" + trustedFileNameForStorage;
            string messageError = "";

            int findingUsers = 0;
            int UpdatedUsers = 0;

            try
            {
                // Abrir archivo Excel
                using (var workBook = new XLWorkbook(filepath))
                {
                    var pages = workBook.Worksheets.Count;


                    foreach (var worksheet in workBook.Worksheets)
                    {

                        if (worksheet != null)
                        {
                            var headerRow = worksheet.Row(1);
                            bool headersFound = false;

                            foreach (var cell in headerRow.Cells())
                            {
                                if (cell.Value.ToString() == "Número de nómina" && cell.Address.ColumnLetter == "J" &&
                                    headerRow.Cell(cell.Address.ColumnNumber + 1).Value.ToString() == "Nombre" && headerRow.Cell(cell.Address.ColumnNumber + 1).Address.ColumnLetter == "K")
                                {
                                    headersFound = true;
                                    break;
                                }
                            }

                            if (!headersFound && worksheet.Name != "HC")
                            {
                                continue; 
                            }


                            IEnumerable<User> users = await _supervisorMobilityRepository.GetAllUsersAsync();


                            int i = 0;
                            var rows = worksheet.Rows();
                            foreach (var row in rows.SkipWhile(r => r.RowNumber() < 4))
                            {

                                var cellPayroll = row.Cell("J");
                                var cellName = row.Cell("K");
                          
                                int retries = 0;
                                const int maxRetries = 5;
                                TimeSpan retryInterval = TimeSpan.FromSeconds(5);

                                while (retries < maxRetries)
                                {
                                    try
                                    {
                                        bool allCellsAreEmpty = true;

                                        // Columna 9: Extraer el numero de nomina y procesar valor
                                        string Payroll = cellPayroll.Value.ToString() != "" ? cellPayroll.Value.ToString() : "";

                                        // Columna 10: Extraer el nombre de la persona
                                        string NameUser = cellName.Value.ToString() != "" ? cellName.Value.ToString() : "";


                                        for (char col = 'B'; col <= 'Z'; col++)
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
                                            //break;
                                            continue;
                                        }

                                        User? usrLocate = users.FirstOrDefault(u => u.Name == NameUser);
                                        if (usrLocate != null)
                                        {
                                            if (usrLocate.Payroll is null )
                                            {
                                                UpdatedUsers++;
                                                usrLocate.Payroll = int.Parse(Payroll);


                                                await _supervisorMobilityRepository.UpdateUser(_mapper.Map<UsersForUpdateDto>(usrLocate), (int)usrLocate.Payroll);
                                                var rs = await _supervisorMobilityRepository.SaveChangesAsync();

                                                if (rs)
                                                {
                                                    Console.WriteLine("Usuario actualizado con exito");
                                                    Debug.WriteLine("Usuario actualizado con exito");
                                                }
                                            }

                                        Console.WriteLine($"User {NameUser} encontrado id: {usrLocate?.UserId}");
                                        Debug.WriteLine($"User {NameUser} encontrado id: {usrLocate?.UserId}");

                                        Console.WriteLine($"Intento {retries + 1} Línea [{i}] completado");
                                        Debug.WriteLine($"Intento {retries + 1} Línea [{i}] completado");

                                            findingUsers++;
                                        }

                                        break; // Operación exitosa
                                    }
                                    catch (Exception ex)
                                    {
                                        retries++;
                                        Debug.WriteLine($"Intento {retries} en Línea [{i}] falló: {ex.Message}");

                                        if (retries == maxRetries)
                                        {
                                            messageError += $"Error en la fila [{i}]. Verifica el documento.\n";
                                        }

                                        await Task.Delay(retryInterval);
                                    }
                                }



                                i++;
                            }

                            Debug.WriteLine("Pagina procesada");
                            Console.WriteLine("Pagina procesada");
                        }

                            break;
                    }
                }
            }//end try
            catch (Exception ex)
            {
                Debug.WriteLine($"Error en Using Woorkbook {ex.ToString()}");
            }//end trycatch to add excel to list
            finally
            {
                Debug.WriteLine($"Users Found: {findingUsers}");
                Console.WriteLine($"Users Found: {findingUsers}"); 
                Debug.WriteLine($"Users Updated: {UpdatedUsers}");
                Console.WriteLine($"Users Updated: {UpdatedUsers}");

            }

            var objectRetur = new { FoundUsers = findingUsers, UpdatedUsers = UpdatedUsers };


            return Ok(objectRetur);
        }




    }

}
