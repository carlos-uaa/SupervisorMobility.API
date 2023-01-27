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
                return NotFound();
            }

            if (!await _supervisorMobilityRepository.AreaExistAsync(newAssyChart.AreaId))
            {
                return NotFound();
            }

            if (!await _supervisorMobilityRepository.DistributionExistsAsync(newAssyChart.DistributionId))
            {
                return NotFound();
            }

            //Object new operation
            var newOperation = _mapper.Map<Operation>(new { newAssyChart.CodeOperation, newAssyChart.DescriptionOperation, newAssyChart.IsActiveOperation });
            //create operatin
            await _assyChartService.CreateOperationAsync(newAssyChart.AreaId, newAssyChart.DistributionId, newOperation);

            //operation to get id
            var createdOperation = _mapper.Map<OperationWithoutNavigationPropertiesDto>(newOperation);

            var finalAssyChart = _mapper.Map<AssyChart>(newAssyChart);

            await _assyChartService.CreateAssyChartAsync(finalAssyChart);

            var createdAssychart =
                _mapper.Map<AssyChartWithoutNavigationProperties>(finalAssyChart);


            return NotFound();

        }


    }
}
