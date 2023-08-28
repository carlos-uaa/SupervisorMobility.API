using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SpreadsheetLight;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.DataAccess.Entities;
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


        public AssyChartController(IAssyChartService assyChartService, ISupervisorMobilityRepository supervisorMobilityRepository,
            IMapper mapper)
        {
            _supervisorMobilityRepository = supervisorMobilityRepository ??
                throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
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
                foreach (var RouteInList in newAssyChart.RoutesProductsAssyChart)
                {
                    RoutesInAssyChart.Add(RouteInList);
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

                    var finalRouteAssyChart = _mapper.Map<RouteProductAssyChart>(elementInList);

                    await _supervisorMobilityRepository.AssychartCreateRoute(finalRouteAssyChart);


                    _supervisorMobilityRepository.AssychartAddRoute(finalAssyChart, finalRouteAssyChart);
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


        [HttpGet]
        public async Task<ActionResult<IEnumerable<AssyChartWhitInfo>>> GetAllAssyCharts()
        {

            var allAssyCharts = await _supervisorMobilityRepository.GetAllAssyChartsAsync();

            return Ok(_mapper.Map<IEnumerable<AssyChartWhitInfo>>(allAssyCharts));
        }

        [HttpGet("{assychartId}")]
        public async Task<ActionResult<AssyChartForUpdateDto>> GetAssyChart(int assychartId)
        {

            var asssychart = await _supervisorMobilityRepository.GetAssyChartAsync(assychartId);

            return Ok(_mapper.Map<AssyChartForUpdateDto>(asssychart));
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

                                var finalRouteAssyChart = _mapper.Map<RouteProductAssyChart>(elementInList);

                                await _supervisorMobilityRepository.AssychartCreateRoute(finalRouteAssyChart);


                                _supervisorMobilityRepository.AssychartAddRoute(finalAssyChart, finalRouteAssyChart);
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
            List<RouteProductAssyChart> RoutesInAssyChart = new List<RouteProductAssyChart>();
            List<RouteProductAssyChart> RoutesWithoutChanges = new List<RouteProductAssyChart>();
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
                foreach (var RouteInList in AssyCharttoUpdate.RoutesProductsAssyChart)
                {

                    if(RouteInList.AssyChardId != 0 && RouteInList.RouteProductAssyChartId != 0)
                    {
                        var RouteInDb = await _supervisorMobilityRepository.GetAssyChartRouteItemAsync(RouteInList.RouteProductAssyChartId);

                        if (RouteInDb.GOS != RouteInList.GOS || RouteInDb.CCP != RouteInList.CCP || RouteInDb.HOE != RouteInList.HOE)
                        {
                            _mapper.Map(RouteInList, RouteInDb);
                            RoutesInAssyChart.Add(RouteInDb);
                        }
                        else
                        {
                            RoutesWithoutChanges.Add(RouteInDb);
                        }
                    }
                    else
                    {
                        var SearchRouteInDb = await _supervisorMobilityRepository.TryFindGetAssyChartRouteItemAsync(assychartId, (int)RouteInList.ProductId);

                        if(SearchRouteInDb != null)
                        {
                            if (SearchRouteInDb.GOS != RouteInList.GOS || SearchRouteInDb.CCP != RouteInList.CCP || SearchRouteInDb.HOE != RouteInList.HOE)
                            {
                                RouteInList.RouteProductAssyChartId = SearchRouteInDb.RouteProductAssyChartId;
                                _mapper.Map(RouteInList, SearchRouteInDb);
                                RoutesInAssyChart.Add(SearchRouteInDb);
                            }
                            else
                            {
                                RoutesWithoutChanges.Add(SearchRouteInDb);
                            }
                        }
                        else
                        {
                            RoutesForCreate.Add(_mapper.Map<RouteProductAssyChartForCreationDto>(RouteInList));

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
                await _supervisorMobilityRepository.AssyChartRemoveAllRoutes(assyChartEntity);

                foreach (var RouteInList in RoutesInAssyChart)
                {
                    _supervisorMobilityRepository.AssychartAddRoute(assyChartEntity, RouteInList);
                }

                foreach (var RouteRestore in RoutesWithoutChanges)
                {
                    _supervisorMobilityRepository.AssychartAddRoute(assyChartEntity, RouteRestore);
                }

                foreach (RouteProductAssyChartForCreationDto elementInList in RoutesForCreate)
                {
                    elementInList.AssyChardId = assyChartEntity.AssyChardId;

                    var finalRouteAssyChart = _mapper.Map<RouteProductAssyChart>(elementInList);

                    await _supervisorMobilityRepository.AssychartCreateRoute(finalRouteAssyChart);

                    _supervisorMobilityRepository.AssychartAddRoute(assyChartEntity, finalRouteAssyChart);
                }

            }



            return Ok();

        }


        //[EnableCors("Cors")]
        [HttpGet("DownloadAssyChartFormat")]
        public async Task<IActionResult> DownloadAssyChartFormat()
        {

            MemoryStream ms = new MemoryStream(6000 * 65536);
            SLDocument ws = new SLDocument();

            ws.RenameWorksheet(SLDocument.DefaultFirstSheetName, "Users Bulk");

            //ROW Data identificators

            ws.SetCellValue("A1", "PlantId");
            ws.SetCellValue("B1", "This Field For Id of plant");

            ws.SetCellValue("D1", "PlantCode");
            ws.SetCellValue("E1", "This Field For Code");

            ws.SetCellValue("G1", "PlantDescription");
            ws.SetCellValue("H1", "This Field For Description");


            //ROW Data identificators

            ws.SetCellValue("A2", "AssyChartId");
            ws.SetCellValue("B2", "AssyChart is Active");
            ws.SetCellValue("C2", "GOS");
            ws.SetCellValue("D2", "CCP");
            ws.SetCellValue("E2", "HOE");
            ws.SetCellValue("F2", "CreationDate");
            ws.SetCellValue("G2", "ModificationDate");
            ws.SetCellValue("H2", "ProductId");
            ws.SetCellValue("I2", "ProductCode");
            ws.SetCellValue("J2", "ProductDescription");
            ws.SetCellValue("K2", "ProductIsActive");
            ws.SetCellValue("L2", "AreaId");
            ws.SetCellValue("M2", "AreaCode");
            ws.SetCellValue("N2", "AreaDescription");
            ws.SetCellValue("O2", "AreaIsActive");
            ws.SetCellValue("P2", "OperationId");
            ws.SetCellValue("Q2", "OperationCode");
            ws.SetCellValue("R2", "OperationDescription");
            ws.SetCellValue("S2", "OperationIsActive");
            ws.SetCellValue("T2", "DistributionId");
            ws.SetCellValue("U2", "DistributionCode");
            ws.SetCellValue("V2", "DistributionDescription");
            ws.SetCellValue("W2", "DistributionIsActive");

            ws.SetCellValue("A3", "This Field For Id of AssyChart");
            ws.SetCellValue("B3", "This Field For True|False");
            ws.SetCellValue("C3", "This Field For path of GOS");
            ws.SetCellValue("D3", "This Field For path of CCP");
            ws.SetCellValue("E3", "This Field For path of HOE");
            ws.SetCellValue("F3", "This Field For Creation Date");
            ws.SetCellValue("G3", "This Field For Modification Date");
            ws.SetCellValue("H3", "This Field For id of Product");
            ws.SetCellValue("I3", "This Field For code of Product");
            ws.SetCellValue("J3", "This Field For description of Product");
            ws.SetCellValue("K3", "This Field For Product Is Active (True|False)");
            ws.SetCellValue("L3", "This Field For Area Id");
            ws.SetCellValue("M3", "This Field For Code of Area");
            ws.SetCellValue("N3", "This Field For Descripcion of Area");
            ws.SetCellValue("O3", "This Field For Area Is Active (True|False)");
            ws.SetCellValue("P3", "This Field For Id of Operation");
            ws.SetCellValue("Q3", "This Field For Code of Operation");
            ws.SetCellValue("R3", "This Field For Description of Operation");
            ws.SetCellValue("S3", "This Field For Operation Is Active (True|False)");
            ws.SetCellValue("T3", "This Field For Distribution Id");
            ws.SetCellValue("U3", "This Field For Code of Distirbution");
            ws.SetCellValue("V3", "This Field For Description of Distribution");
            ws.SetCellValue("W3", "This Field For DistributionIsActive (True|False)");


            ws.SaveAs(ms);

            ms.Position = 0;

            var res = File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AssyChartFormat.xlsx");
            res.EnableRangeProcessing = true;
            return res;

        }//end download file function 

    }
}
