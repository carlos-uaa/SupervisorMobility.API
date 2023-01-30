using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.AssyChart;
using SupervisorMobility.API.Models.OperationDtos;
using SupervisorMobility.API.Services;

namespace SupervisorMobility.API.Controllers
{
    [Route("api/assycharts")]
    [ApiController]
    public class AssyChartController: ControllerBase
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
        public async Task<ActionResult<AssyChartWithoutNavigationProperties>> CreateAssyChart(AssyChartForCreationDto newAssyChart)
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
            newOperation.Code = newAssyChart.CodeOperation;
            newOperation.Description = newAssyChart.DescriptionOperation;
            newOperation.IsActive = newAssyChart.IsActiveOperation;

            var finalOperation = _mapper.Map<Operation>(newOperation);

            await _assyChartService.CreateOperationAsync(newAssyChart.AreaId, newAssyChart.DistributionId, finalOperation);
            
            //operation to get id
            var createdOperationAndGetId = _mapper.Map<OperationWithoutNavigationPropertiesDto>(finalOperation);

            
            var finalAssyChart = _mapper.Map<AssyChart>(newAssyChart);
            finalAssyChart.OperationId = createdOperationAndGetId.OperationId;
            await _assyChartService.CreateAssyChartAsync(finalAssyChart);

            var createdAssychartToReturn = _mapper.Map<AssyChartWithoutNavigationProperties>(finalAssyChart);


            return CreatedAtRoute("GetAssyChart",
                new
                {
                    assychardid = createdAssychartToReturn.AssyChardId
                },
                createdAssychartToReturn);

        }


    }
}
