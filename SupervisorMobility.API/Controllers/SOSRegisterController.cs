using AutoMapper;
using DocumentFormat.OpenXml.Office2010.Drawing;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.SOS_Review;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.AreaDtos;
using SupervisorMobility.API.Models.JobObservationDtos;
using SupervisorMobility.API.Models.SOSReviewDtos;
using SupervisorMobility.API.Services;
using System.Diagnostics;

namespace SupervisorMobility.API.Controllers
{

    [Route("api/SOSReview")]
    [ApiController]

    public class SOSRegisterController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ISupervisorMobilityRepository _supervisorMobilityRepository;
        private readonly IAssyChartService _assyChartService;

        public SOSRegisterController(ISupervisorMobilityRepository supervisorMobilityRepository, IAssyChartService assyChartService,
            IMapper mapper)
        {
            _supervisorMobilityRepository = supervisorMobilityRepository ??
                throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _assyChartService = assyChartService ??
                throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

      

        [HttpGet("Registers/{SOSid}")]
        public async Task<ActionResult<IEnumerable<SOSReviewsRegisterDto>>> SOSReviewRegisters(int SOSid, bool includeCollections = false)
        {
            var SOS_Reviews = await _supervisorMobilityRepository.GetAllSOSReviewsRegisters(SOSid);

            if (includeCollections)
            {
                return Ok(_mapper.Map<IEnumerable<SOSReviewsRegisterDto>>(SOS_Reviews));
            }
            else
            {
                return Ok(_mapper.Map<IEnumerable<SOSReviewsRegisterDto>>(SOS_Reviews));
            }

        }//end get all registers
      


        [HttpPost("Registers/{SOSid}")]
        public async Task<ActionResult<SOSReviewWithAllDto>> CreateSOSRegister(int SOSid, int month, int year, JobObservationForCreationDto JobEntity)
        {

            var finalJob = _mapper.Map<JobObservation>(JobEntity);

            if (finalJob.OperationId == 0)
            {
                finalJob.OperationId = null;
            }

            if (finalJob.OperatorId == 0)
            {
                finalJob.OperatorId = null;
            }

            if (finalJob.StartDate.HasValue)
            {
                if (JobEntity.StartDate.Value.DayOfWeek == DayOfWeek.Saturday)
                {
                    finalJob.StartDate = finalJob.StartDate.Value.AddDays(2);
                }
                else if (JobEntity.StartDate.Value.DayOfWeek == DayOfWeek.Sunday)
                {
                    finalJob.StartDate = finalJob.StartDate.Value.AddDays(1);
                }
            }

            finalJob.PlannedStartDate = finalJob.StartDate;

            _supervisorMobilityRepository.AddJobObservation(finalJob);
            await _supervisorMobilityRepository.SaveChangesAsync();

            SOSRegisterJobObservation finalSOSReg = new();

            finalSOSReg.SOSReviewProgramid = SOSid;
            finalSOSReg.Month = month;
            finalSOSReg.Year = year;
            finalSOSReg.JobObservationId = finalJob.JobObservationId;
            finalSOSReg.OperationId = finalJob.OperationId;


            var result = await _supervisorMobilityRepository.AddSOSReviewRegister(finalSOSReg);


            if (result > 0)
            {
                await _supervisorMobilityRepository.SaveChangesAsync();

                var createdRegisterToReturn =
                    _mapper.Map<SOSReviewsRegisterDto>(finalSOSReg);

                return Ok(createdRegisterToReturn);
            }
            else
            {
                return NotFound();
            }


        }//end post create register


    }//end main clas
}//end namespace
