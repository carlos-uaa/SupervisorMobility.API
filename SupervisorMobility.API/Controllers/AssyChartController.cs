using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SpreadsheetLight;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.Models.AssyChart;
using SupervisorMobility.API.Services;

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
            if (!await _supervisorMobilityRepository.PlantExistAsync(newAssyChart.PlantId))
            {
                return NotFound("No Planta");
            }

            if (!await _supervisorMobilityRepository.AreaExistAsync(newAssyChart.AreaId))
            {
                return NotFound("No Area");
            }

            if (!await _supervisorMobilityRepository.DistributionExistsAsync(newAssyChart.DistributionId))
            {
                return NotFound("No Distributio");
            }


            var finalAssyChart = await _assyChartService.CreateAssyChartAsync(newAssyChart);


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
            var assyChartsForPlant = await _supervisorMobilityRepository.GetAssyChartByPlantAsync(plantId);
            return Ok(_mapper.Map<IEnumerable<AssyChartWhitInfo>>(assyChartsForPlant));
        }

        [HttpGet("plant/{plantId}/area/{areaId}")]
        public async Task<ActionResult<IEnumerable<AssyChartWhitInfo>>> GetAssyChartsOfArea(int plantId, int areaId)
        {
            var assyChartsForPlant = await _supervisorMobilityRepository.GetAssyChartByAreaAsync(plantId, areaId);
            return Ok(_mapper.Map<IEnumerable<AssyChartWhitInfo>>(assyChartsForPlant));
        }

        [HttpGet("plant/{plantId}/area/{areaId}/distribution/{distributionId}")]
        public async Task<ActionResult<IEnumerable<AssyChartWhitInfo>>> GetAssyChartsOfDistribution(int plantId, int areaId, int distributionId)
        {
            var assyChartsForPlant = await _supervisorMobilityRepository.GetAssyChartByDistributionAsync(plantId, areaId, distributionId);
            return Ok(_mapper.Map<IEnumerable<AssyChartWhitInfo>>(assyChartsForPlant));
        }

        [HttpGet("plant/{plantId}/area/{areaId}/distribution/{distributionId}/operation/{operationId}")]
        public async Task<ActionResult<AssyChartForUpdateDto>> GetAssyChartForJobObservation(int plantId, int areaId, int distributionId, int operationId)
        {

            var asssychart = await _supervisorMobilityRepository.GetAssyChartForJobObservationAsync(plantId, areaId, distributionId, operationId);

            return Ok(_mapper.Map<AssyChartForUpdateDto>(asssychart));
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
            if (!await _supervisorMobilityRepository.PlantExistAsync(AssyCharttoUpdate.PlantId))
            {
                return NotFound("No Planta");
            }

            if (!await _supervisorMobilityRepository.AreaExistAsync(AssyCharttoUpdate.AreaId))
            {
                return NotFound("No Area");
            }

            if (!await _supervisorMobilityRepository.DistributionExistsAsync(AssyCharttoUpdate.DistributionId))
            {
                return NotFound("No Distributio");
            }

            if (!await _supervisorMobilityRepository.OperationExistsAsync(AssyCharttoUpdate.OperationId))
            {
                return NotFound("No Operation");
            }

            var assyChartEntity = await _supervisorMobilityRepository.GetAssyChartAsync(assychartId);

            if (assyChartEntity == null)
            {
                return NotFound("AssyChart Not Found");
            }

            await _assyChartService.UpdateAssyChartAsync(AssyCharttoUpdate, assyChartEntity);

            return Ok();

        }


        [EnableCors("Cors")]
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
