using AutoMapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Entities.SOS.History;
using SupervisorMobility.API.DataAccess.Entities.TreeStruct;
using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.DataAccess.Services.TreeServices;
using SupervisorMobility.API.Entities.CDMS;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.CommentaryDtos;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.SOS.EquipmentDtos;
using SupervisorMobility.API.Models.SOS.MaterialDtos;
using SupervisorMobility.API.Models.SOS.SOSAnalysisDtos;
using SupervisorMobility.API.Models.SOS.SOSAnalysisLogbookDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos;
using SupervisorMobility.API.Models.SOS.SOSTimeDtos;
using SupervisorMobility.API.Models.SOS.ToolDtos;
using SupervisorMobility.API.Models.SOSReviewDtos;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using FuzzyString;
using DocumentFormat.OpenXml.EMMA;

namespace SupervisorMobility.API.Controllers.SOS_Controllers
{
    [Route("api/AllTest")]
    [ApiController]
    public class AllTestController : Controller
    {
        private readonly ISOS_ProcessRepository _ProcessRepository;
        private readonly IMapper _mapper;
        private readonly IServiceProvider _serviceProvider;
        private readonly IWebHostEnvironment _env;
        public AllTestController(IWebHostEnvironment env, IMapper mapper, ISOS_ProcessRepository repository, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _ProcessRepository = repository;
            _mapper = mapper ??
                  throw new ArgumentNullException(nameof(mapper));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

      

        [HttpPost("TestFindPath")]
        public async Task<ActionResult> Test(int? plantid, int? areaid, int? distributionid, int? productid)
        {
            var itemToReturn = new object();
            int p_id = 4;
            int a_id = 36;
            int d_id = 1436;
            int m_id = 3;

            if(plantid != null && areaid != null && distributionid != null && productid != null)
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

                nodoHoe =  _treeService.EncontrarNodoMejorCoincidencia(rootNodeHOE, coincidenciasplanta, "produccion", coincidenciasAreas.Value, coincidenciasDistributions.Value, coincidenciaProduct);
                if(nodoHoe != null)
                {
                    Console.WriteLine($"Nodo [HOE] c: {nodoHoe?.Ruta}" );
                    Debug.WriteLine($"Nodo [HOE]: {nodoHoe?.Ruta}" );
                }
                else
                {
                    Console.WriteLine($"[HOE] No encontrado :c  " );
                    Debug.WriteLine($"HOE No encontrado :c  " );

                    
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

    }

}
