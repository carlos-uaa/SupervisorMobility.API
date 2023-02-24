using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.AreaDtos;
using SupervisorMobility.API.Models.AssyChart;
using SupervisorMobility.API.Models.OperationDtos;
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
        public async Task<ActionResult<AssyChartWithoutNavigationProperties>> CreateAssyChart(AssyChartForCreationRecived newAssyChart)
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

            OperationForCreationDto newOperation = new();
            newOperation.Code = newAssyChart.OperationCode;
            newOperation.Description = newAssyChart.OperationDescription;
            newOperation.IsActive = newAssyChart.OperationIsActive;

            var finalOperation = _mapper.Map<Operation>(newOperation);

            await _assyChartService.CreateOperationAsync(newAssyChart.AreaId, newAssyChart.DistributionId, finalOperation);
      

            AssyChartForCreation finalAssyChart = new AssyChartForCreation()
            {
                GOS = newAssyChart.GOS,
                CCP = newAssyChart.CCP,
                HOE = newAssyChart.HOE,
                ProductId = newAssyChart.ProductId,
                PlantId = newAssyChart.PlantId,
                AreaId = newAssyChart.AreaId,
                DistributionId = newAssyChart.DistributionId,
                OperationId = finalOperation.OperationId,
                CreationDate = DateTime.Now,
                ModificationDate = DateTime.UtcNow
            };

            await _assyChartService.CreateAssyChartAsync(finalAssyChart);


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

        [HttpGet("byplantid/{plantId}")]
        public async Task<ActionResult<IEnumerable<AssyChartWhitInfo>>> GetAssyChartsOfPlant(int plantId)
        {
            var assyChartsForPlant = await _supervisorMobilityRepository.GetAssyChartByPlantAsync(plantId);
            return Ok(_mapper.Map<IEnumerable<AssyChartWhitInfo>>(assyChartsForPlant));
        }

        [HttpGet("{assychartId}")]
        public async Task<ActionResult<AssyChartForUpdateDto>> GetAssyChart(int assychartId)
        {

            var asssychart = await _supervisorMobilityRepository.GetAssyChartAsync(assychartId);

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

            _supervisorMobilityRepository.DeleteAssyChartAsync(assychart);
            await _supervisorMobilityRepository.SaveChangesAsync();

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

            _mapper.Map(AssyCharttoUpdate, assyChartEntity);

            await _supervisorMobilityRepository.SaveChangesAsync();

            return Ok();

        }

    }
}
