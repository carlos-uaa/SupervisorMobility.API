using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.StaticFiles;
using SpreadsheetLight;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.Paths;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.AssyChart;
using SupervisorMobility.API.Models.OperationDtos;
using SupervisorMobility.API.Models.RouteProductAssyChartDtos;
using SupervisorMobility.API.Models.Users;
using SupervisorMobility.API.Services;
using System.Diagnostics;
using System.Linq.Expressions;

namespace SupervisorMobility.API.Controllers
{
    [Route("api/assycharts")]
    [ApiController]
    public class AssyChartController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IAssyChartService _assyChartService;
        private readonly ISupervisorMobilityRepository _supervisorMobilityRepository;

        private readonly IWebHostEnvironment _env;

        public AssyChartController(IWebHostEnvironment env, IAssyChartService assyChartService, ISupervisorMobilityRepository supervisorMobilityRepository,
            IMapper mapper)
        {
            _supervisorMobilityRepository = supervisorMobilityRepository ??
                throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
            _env = env ??
                throw new ArgumentNullException(nameof(env));
            _assyChartService = assyChartService;

        }



        [HttpPost]
        public async Task<ActionResult<AssyChartWithoutNavigationProperties>> CreateAssyChart(AssyChartForCreation newAssyChart)
        {
            List<RouteProductAssyChartForCreationDto> RoutesInAssyChart = new List<RouteProductAssyChartForCreationDto>();
            bool haveRoutes = false;


            if (newAssyChart.PlantId == 0)
            {
                newAssyChart.PlantId = null;
            }
            else if (newAssyChart.PlantId != null)
            {
                if (!await _supervisorMobilityRepository.PlantExistAsync((int)newAssyChart.PlantId))
                {
                    return NotFound("No Planta");
                }
            }


            if (newAssyChart.AreaId == 0)
            {
                newAssyChart.AreaId = null;
            }
            else if (newAssyChart.AreaId != null)
            {
                if (!await _supervisorMobilityRepository.AreaExistAsync((int)newAssyChart.AreaId))
                {
                    return NotFound("No Area");
                }
            }


            if (newAssyChart.DistributionId == 0)
            {
                newAssyChart.DistributionId = null;
            }
            else if (newAssyChart.DistributionId != null)
            {
                if (!await _supervisorMobilityRepository.DistributionExistsAsync((int)newAssyChart.DistributionId))
                {
                    return NotFound("No Distributio");
                }

            }



            if (newAssyChart.RoutesProductsAssyChart != null && newAssyChart.RoutesProductsAssyChart?.Count > 0)
            {
                haveRoutes = true;
                foreach (var CodePathInList in newAssyChart.RoutesProductsAssyChart)
                {
                    RoutesInAssyChart.Add(CodePathInList);
                }
                newAssyChart.RoutesProductsAssyChart = null;
            }


            if (newAssyChart.OperationId == 0)
            {
                newAssyChart.OperationId = null;
            }
            else if (newAssyChart.OperationId != null)
            {
                if (!await _supervisorMobilityRepository.OperationExistsAsync((int)newAssyChart.OperationId))
                {
                    return NotFound("No Operation");
                }
            }

            var finalAssyChart = await _assyChartService.CreateAssyChartAsync(newAssyChart);


            if (haveRoutes)
            {
                foreach (RouteProductAssyChartForCreationDto elementInList in RoutesInAssyChart)
                {
                    elementInList.AssyChardId = finalAssyChart.AssyChardId;

                    var finalRouteAssyChart = _mapper.Map<SOSCodePath>(elementInList);

                    await _supervisorMobilityRepository.AssychartCreateCodePath(finalRouteAssyChart);


                    _supervisorMobilityRepository.AssychartAddCodePath(finalAssyChart, finalRouteAssyChart);
                }

            }

            await _supervisorMobilityRepository.SaveChangesAsync();

            return Ok(finalAssyChart);
            //return CreatedAtRoute("GetAssyChart",
            //    new
            //    {
            //        assychardid = createdAssychartToReturn.AssyChardId
            //    },
            //    createdAssychartToReturn);
        }

        [HttpPost("CodePath")]
        public async Task<ActionResult<AssyChartWhitInfo>> CreatePathRoute(RouteProductAssyChartForCreationDto _newCodePath)
        {

            var assyChartEntity = await _supervisorMobilityRepository.GetAssyChartAsync((int)_newCodePath.AssyChardId);


            var finalRouteAssyChart = _mapper.Map<SOSCodePath>(_newCodePath);

            await _supervisorMobilityRepository.AssychartCreateCodePath(finalRouteAssyChart);

            _supervisorMobilityRepository.AssychartAddCodePath(assyChartEntity, finalRouteAssyChart);

            return Ok(assyChartEntity);
        }

        [HttpGet("CodePath")]
        public async Task<ActionResult<IEnumerable<RouteProductAssyChartWithNavigations>>> GetAllPathsRoute()
        {
            var allAssyCharts = await _supervisorMobilityRepository.GetAllCodePathsAsync();

            return Ok(_mapper.Map<IEnumerable<RouteProductAssyChartWithNavigations>>(allAssyCharts));

      
        }


        [HttpGet("CodePath/{CodePathId}")]
        public async Task<ActionResult<IEnumerable<RouteProductAssyChartWithNavigations>>> GetCodePathOnly(int CodePathId)
        {
            var CodePath = await _supervisorMobilityRepository.GetCodePathItemAsync(CodePathId);

            return Ok(_mapper.Map<RouteProductAssyChartWithNavigations>(CodePath));

        }



        [HttpPut("CodePath/{CodePathId}")]
        public async Task<ActionResult<RouteProductAssyChartWithNavigations>> UpdateCodePathOnly(int CodePathId, RouteProductAssyChartForUpdateDto CodePathForUpdate)
        {
            var CodePathInDB = await _supervisorMobilityRepository.GetCodePathItemAsync(CodePathId);

            _mapper.Map(CodePathForUpdate, CodePathInDB);


            var assyChartEntity = await _supervisorMobilityRepository.GetAssyChartAsync((int)CodePathForUpdate.AssyChardId);

          

            _supervisorMobilityRepository.AssychartAddCodePath(assyChartEntity, CodePathInDB);


            await _supervisorMobilityRepository.SaveChangesAsync();

            return (_mapper.Map<RouteProductAssyChartWithNavigations>(CodePathInDB));
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<AssyChartWhitInfo>>> GetAllAssyCharts()
        {

            var allAssyCharts = await _supervisorMobilityRepository.GetAllAssyChartsAsync();

            return Ok(_mapper.Map<IEnumerable<AssyChartWhitInfo>>(allAssyCharts));
        }

        [HttpGet("{assychartId}")]
        public async Task<ActionResult<AssyChartWhitInfo>> GetAssyChart(int assychartId)
        {

            var asssychart = await _supervisorMobilityRepository.GetAssyChartAsync(assychartId);

            return Ok(_mapper.Map<AssyChartWhitInfo>(asssychart));
        }

        [HttpGet("plant/{plantId}")]
        public async Task<ActionResult<IEnumerable<AssyChartWhitInfo>>> GetAssyChartsOfPlant(int plantId)
        {
            var assyChartsForPlant = await _supervisorMobilityRepository.GetAllAssyChartsByPlantAsync(plantId);
            return Ok(_mapper.Map<IEnumerable<AssyChartWhitInfo>>(assyChartsForPlant));
        }

        [HttpGet("plant/{plantId}/area/{areaId}")]
        public async Task<ActionResult<IEnumerable<AssyChartWhitInfo>>> GetAssyChartsOfArea(int plantId, int areaId)
        {
            var assyChartsForPlant = await _supervisorMobilityRepository.GetAllAssyChartsByAreaAsync(plantId, areaId);
            return Ok(_mapper.Map<IEnumerable<AssyChartWhitInfo>>(assyChartsForPlant));
        }

        [HttpGet("plant/{plantId}/area/{areaId}/distribution/{distributionId}/list")]
        public async Task<ActionResult<IEnumerable<AssyChartWhitInfo>>> GetAssyChartsOfDistribution(int plantId, int areaId, int distributionId)
        {
            var assyChartsForPlant = await _supervisorMobilityRepository.GetAllAssyChartsByDistributionAsync(plantId, areaId, distributionId);
            return Ok(_mapper.Map<IEnumerable<AssyChartWhitInfo>>(assyChartsForPlant));
        }

        [HttpGet("plant/{plantId}/area/{areaId}/distribution/{distributionId}/operation/{operationId}")]
        public async Task<ActionResult<AssyChartWhitInfo>> GetAssyChartAdvance(int plantId, int areaId, int distributionId, int operationId)
        {

            var asssychart = await _supervisorMobilityRepository.GetAssyChartAdvanceAsync(plantId, areaId, distributionId, operationId);

            return Ok(_mapper.Map<AssyChartWhitInfo>(asssychart));
        }

        [HttpGet("plant/{plantId}/area/{areaId}/distribution/{distributionId}/one")]
        public async Task<ActionResult<AssyChartWhitInfo>> GetAssyChartForJobObservation(int plantId, int areaId, int distributionId)
        {

            var asssychart = await _supervisorMobilityRepository.GetAssyChartForJobObservationAsync(plantId, areaId, distributionId);

            return Ok(_mapper.Map<AssyChartWhitInfo>(asssychart));
        }


        [HttpGet("madeAssyChartsAllDistributionsExist")]
        public async Task<ActionResult<AssyChartWhitInfo>> madeAssyCharts()
        {

            IEnumerable<Distribution> _distributions = await _supervisorMobilityRepository.GetAllDistributions();

            int countDistMade = 0;
            foreach(var Dist in _distributions)
            {

            
                int maxRetries = 5; // Número máximo de intentos
                TimeSpan retryInterval = TimeSpan.FromSeconds(5); // Intervalo de tiempo entre intentos (5 segundos en este caso)
                int retries = 1;


                while (retries < maxRetries)
                {
                    try
                    {
                        // Intenta realizar la operación aquí
                        
                        var area = await _supervisorMobilityRepository.GetAreaOnlyIdAsync(Dist.AreaId);
                       
                        var planta = await _supervisorMobilityRepository.GetPlantOnlyIdAsync(area.PlantId);

                        AssyChart? assychart = await _supervisorMobilityRepository.GetAssyChartForJobObservationAsync(planta.PlantId, area.AreaId, Dist.DistributionId);

                        if (assychart is null)
                        {
                            Debug.WriteLine($" assychart NO existe: {countDistMade}");
                            AssyChartForCreation newAssyChart = new();

                            //newAssyChart.ProductId = product.ProductId;
                            newAssyChart.DistributionId = Dist.DistributionId;
                            newAssyChart.AreaId = area.AreaId;
                            newAssyChart.PlantId = planta.PlantId;


                            var finalAssyChart = await _assyChartService.CreateAssyChartAsync(newAssyChart);


                            foreach (var ProdInDist in Dist.Products)
                            {

                                RouteProductAssyChartForCreationDto elementInList = new();

                                elementInList.ProductId = ProdInDist.ProductId;
                                elementInList.AssyChardId = finalAssyChart.AssyChardId;
                                elementInList.IsActive = true;

                                var finalRouteAssyChart = _mapper.Map<SOSCodePath>(elementInList);

                                await _supervisorMobilityRepository.AssychartCreateCodePath(finalRouteAssyChart);


                                _supervisorMobilityRepository.AssychartAddCodePath(finalAssyChart, finalRouteAssyChart);
                                Debug.WriteLine($"  Route of `{ProdInDist.Code}` for: {countDistMade} creada");
                            }
                        }
                        else
                        {
                            Debug.WriteLine($" assychart existe: {countDistMade}");
                            await _supervisorMobilityRepository.SaveChangesAsync();
                        }

                        retries = 0;
                        // Si la operación tiene éxito, puedes salir del bucle
                        break;
                    }
                    catch (Exception ex)
                    {
                        // Maneja la excepción aquí, si es necesario
                        Console.WriteLine($"Intento {retries + 1} falló: {ex.Message}");

                        // Incrementa el número de intentos
                        retries++;

                        // Espera el intervalo de tiempo antes de volver a intentarlo
                        await Task.Delay(retryInterval);
                    }//tryatch para intentos


                }//while de intentos para la informacion

                countDistMade++;
            }//for de todas las distribuciones

            await _supervisorMobilityRepository.SaveChangesAsync();


            return Ok(countDistMade);
        }

        [HttpDelete("{assychartId}")]
        public async Task<ActionResult> DeleteAssyChart(int assychartId)
        {
            var assychart = await _supervisorMobilityRepository.GetAssyChartAsync(assychartId);

            if (assychart == null)
            {
                return NotFound();
            }

            await _assyChartService.RemoveAssyChartAsync(assychart);

            return Ok();
        }


        [HttpPut("{assychartId}")]
        public async Task<ActionResult> UpdateAssyChart(int assychartId, AssyChartForUpdateDto AssyCharttoUpdate)
        {
            List<SOSCodePath> RoutesInAssyChart = new List<SOSCodePath>();
            List<SOSCodePath> RoutesWithoutChanges = new List<SOSCodePath>();
            List<RouteProductAssyChartForCreationDto> RoutesForCreate = new List<RouteProductAssyChartForCreationDto>();
            bool haveRoutes = false;

            if (AssyCharttoUpdate.PlantId == 0)
            {
                AssyCharttoUpdate.PlantId = null;
            }
            else if (AssyCharttoUpdate.PlantId != null)
            {
                if (!await _supervisorMobilityRepository.PlantExistAsync((int)AssyCharttoUpdate.PlantId))
                {
                    return NotFound("No Planta");
                }
            }


            if (AssyCharttoUpdate.AreaId == 0)
            {
                AssyCharttoUpdate.AreaId = null;
            }
            else if (AssyCharttoUpdate.AreaId != null)
            {
                if (!await _supervisorMobilityRepository.AreaExistAsync((int)AssyCharttoUpdate.AreaId))
                {
                    return NotFound("No Area");
                }
            }


            if (AssyCharttoUpdate.DistributionId == 0)
            {
                AssyCharttoUpdate.DistributionId = null;
            }
            else if (AssyCharttoUpdate.DistributionId != null)
            {
                if (!await _supervisorMobilityRepository.DistributionExistsAsync((int)AssyCharttoUpdate.DistributionId))
                {
                    return NotFound("No Distributio");
                }

            }



            if (AssyCharttoUpdate.OperationId == 0)
            {
                AssyCharttoUpdate.OperationId = null;
            }
            else if (AssyCharttoUpdate.OperationId != null)
            {
                if (!await _supervisorMobilityRepository.OperationExistsAsync((int)AssyCharttoUpdate.OperationId))
                {
                    return NotFound("No Operation");
                }
            }


            var assyChartEntity = await _supervisorMobilityRepository.GetAssyChartAsync(assychartId);

            if (assyChartEntity == null)
            {
                return NotFound("AssyChart Not Found");
            }

            if (AssyCharttoUpdate.RoutesProductsAssyChart != null && AssyCharttoUpdate.RoutesProductsAssyChart?.Count > 0)
            {
                haveRoutes = true;
                foreach (var CodePathInList in AssyCharttoUpdate.RoutesProductsAssyChart)
                {

                    if(CodePathInList.AssyChardId != 0 && CodePathInList.SOSCodePathId != 0)
                    {
                        var CodePathInDB = await _supervisorMobilityRepository.GetCodePathItemAsync(CodePathInList.SOSCodePathId);

                        if (CodePathInDB.GOS != CodePathInList.GOS || 
                            CodePathInDB.CCP != CodePathInList.CCP || 
                            CodePathInDB.HOE != CodePathInList.HOE ||
                            CodePathInDB.CommonDirectionGOS != CodePathInList.CommonDirectionGOS || 
                            CodePathInDB.CommonDirectionCCP != CodePathInList.CommonDirectionCCP || 
                            CodePathInDB.CommonDirectionHOE != CodePathInList.CommonDirectionHOE || 
                            CodePathInDB.Code != CodePathInList.Code)
                        {
                            _mapper.Map(CodePathInList, CodePathInDB);
                            RoutesInAssyChart.Add(CodePathInDB);
                        }
                        else
                        {
                            RoutesWithoutChanges.Add(CodePathInDB);
                        }
                    }
                    else
                    {
                        var SearchCodePathInDb = await _supervisorMobilityRepository.TryFindCodePathItemAsync(assychartId, CodePathInList.Code);

                        if(SearchCodePathInDb != null)
                        {
                            if (SearchCodePathInDb.GOS != CodePathInList.GOS || SearchCodePathInDb.CCP != CodePathInList.CCP || SearchCodePathInDb.HOE != CodePathInList.HOE)
                            {
                                //CodePathInList.SOSCodePathId = SearchCodePathInDb.SOSCodePathId;
                                _mapper.Map(CodePathInList, SearchCodePathInDb);
                                RoutesInAssyChart.Add(SearchCodePathInDb);
                            }
                            else
                            {
                                RoutesWithoutChanges.Add(SearchCodePathInDb);
                            }
                        }
                        else
                        {
                            RoutesForCreate.Add(_mapper.Map<RouteProductAssyChartForCreationDto>(CodePathInList));
                        }

                    }


                }
                AssyCharttoUpdate.RoutesProductsAssyChart = null;
            }

            AssyCharttoUpdate.CreationDate = assyChartEntity.CreationDate;
            AssyCharttoUpdate.ModificationDate = DateTime.Now;

            await _assyChartService.UpdateAssyChartAsync(AssyCharttoUpdate, assyChartEntity);



            if (haveRoutes)
            {
                await _supervisorMobilityRepository.AssyChartRemoveAllCodePaths(assyChartEntity);

                foreach (var CodePathInList in RoutesInAssyChart)
                {
                    _supervisorMobilityRepository.AssychartAddCodePath(assyChartEntity, CodePathInList);
                }

                foreach (var RouteRestore in RoutesWithoutChanges)
                {
                    _supervisorMobilityRepository.AssychartAddCodePath(assyChartEntity, RouteRestore);
                }

                foreach (RouteProductAssyChartForCreationDto elementInList in RoutesForCreate)
                {
                    elementInList.AssyChardId = assyChartEntity.AssyChardId;

                    var finalRouteAssyChart = _mapper.Map<SOSCodePath>(elementInList);

                    await _supervisorMobilityRepository.AssychartCreateCodePath(finalRouteAssyChart);

                    _supervisorMobilityRepository.AssychartAddCodePath(assyChartEntity, finalRouteAssyChart);
                }

            }



            return Ok();

        }


        //[EnableCors("Cors")]
        [HttpGet("DownloadAssyChartFormat")]
        public async Task<IActionResult> DownloadAssyChartFormat()
        {
            string filePath = _env.ContentRootPath + "\\Documents\\Blank_workload.xlsx";

            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(bytes, contentType, Path.GetFileName(filePath));


        }//end download file function 

    }
}
