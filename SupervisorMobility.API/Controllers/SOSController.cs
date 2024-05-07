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
using SupervisorMobility.API.Profiles;
using SupervisorMobility.API.Services;
using System.Diagnostics;

namespace SupervisorMobility.API.Controllers
{

    [Route("api/SOSReview")]
    [ApiController]

    public class SOSController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ISupervisorMobilityRepository _supervisorMobilityRepository;
        private readonly IAssyChartService _assyChartService;

        public SOSController(ISupervisorMobilityRepository supervisorMobilityRepository, IAssyChartService assyChartService,
            IMapper mapper)
        {
            _supervisorMobilityRepository = supervisorMobilityRepository ??
                throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _assyChartService = assyChartService ??
                throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SOSReviewWithAllDto>>> GetAllSos(bool includeNavigation = false, bool includeUsers = false, bool includeSuggestions = false)
        {

            var SOS_Reviews = await _supervisorMobilityRepository.GetAllSOSReviews(includeNavigation,  includeUsers,  includeSuggestions);


            if (includeNavigation || includeUsers || includeSuggestions)
            {
                return Ok(_mapper.Map<IEnumerable<SOSReviewWithAllDto>>(SOS_Reviews));
            }
            else
            {
                return Ok(_mapper.Map<IEnumerable<SOSReviewWithOutDataDto>>(SOS_Reviews));
            }

        }//end get all

       
        [HttpGet("{sosId}", Name = "GetSOS")]
        public async Task<ActionResult<SOSReviewWithAllDto>> GetSOS(int sosId, bool includeNavigation = false, bool includeUsers = false, bool includeSuggestions = false)
        {

            var SOS_Review = await _supervisorMobilityRepository.GetSOSasync(sosId, includeNavigation, includeUsers, includeSuggestions);

            if (SOS_Review == null)
            {
                return NotFound();
            }

            if (includeNavigation || includeUsers || includeSuggestions)
            {
                return Ok(_mapper.Map<SOSReviewWithAllDto>(SOS_Review));
            }
            return Ok(_mapper.Map<SOSReviewWithOutDataDto>(SOS_Review));
        }//end get one

        [HttpPost]
        public async Task<ActionResult<SOSReviewWithAllDto>> CreateSOSReview(
           SOSReviewForCreateDto SOSentity)
        {
            List<User> Users = new List<User>();
            bool haveUsers = false;

            if (SOSentity.Supervisors != null)
            {
                haveUsers = true;
                foreach (var Sub in SOSentity.Supervisors)
                {
                    var usr = await _supervisorMobilityRepository.GetUserAsync(Sub.UserId);
                    if (usr != null)
                    {
                        Users.Add(usr);
                    }
                }

                SOSentity.Supervisors = null;
            }

            var finalSOSReview = _mapper.Map<SOSReviewProgram>(SOSentity);
            var result = await _supervisorMobilityRepository.AddSOSReview(finalSOSReview);

            if (result > 0)
            {
                //añade a SV si el lo creo
                if (haveUsers)
                {
                    foreach (var item in Users)
                    {
                        _supervisorMobilityRepository.SOSReviewAddUser(finalSOSReview, item);
                    }
                }

                await _supervisorMobilityRepository.SaveChangesAsync();

                //Creamos listado de sugerencias de distribucion
                var _alldistributions = await _supervisorMobilityRepository.GetDistributionsForAreaAsync((int)SOSentity.AreaId);

                foreach (var dist in _alldistributions)
                {
                    SOSReviewDistSuggestionForCreateDto distSuggestion = new SOSReviewDistSuggestionForCreateDto();
                    distSuggestion.SOSReviewProgramid = finalSOSReview.SOSid;
                    distSuggestion.DistributionId = dist.DistributionId;

                    SOSReviewDistSuggestion finalDistSugges = _mapper.Map<SOSReviewDistSuggestion>(distSuggestion);

                    _ = await _supervisorMobilityRepository.CreateSOSReviewDistSuggestion(finalDistSugges);

                }




                var createdSOSToReturn =
                    _mapper.Map<SOSReviewWithAllDto>(finalSOSReview);

                return Ok(createdSOSToReturn);
            }
            else
            {
                return NotFound();
            }


        }//end post create 
     
        [HttpPut("{SOSid}")]
        public async Task<ActionResult> UpdateSosReview(int SOSid,
            SOSReviewForUpdateDto sosUpdateEntity)
        {

            var SOS_Entity = await _supervisorMobilityRepository
                .GetSOSasync(SOSid, true, true, true);

            if (SOS_Entity == null)
            {
                return NotFound();
            }

            var result = await _supervisorMobilityRepository.UpdateSOSReview(sosUpdateEntity, SOS_Entity);

            if (result == 0)
            {
                return NotFound();

            }
            else
            {
                //await _supervisorMobilityRepository.SaveChangesAsync();

                return Ok();
            }
            //await _supervisorMobilityRepository.SaveChangesAsync();

        }//end Update 


        [HttpDelete("{SOSid}")]
        public async Task<ActionResult> DeleteSOSReview(int SOSid)
        {

            var SOS_Entity = await _supervisorMobilityRepository
              .GetSOSasync(SOSid, true, true, true);

            var SOS_RegJobs = await _supervisorMobilityRepository.GetAllSOSReviewsRegisters(SOSid);

            if (SOS_Entity == null)
            {
                return NotFound();
            }

            if(SOS_RegJobs?.Count() > 0)
            {
                //Eliminamos todas las jobs 
                foreach(var item in SOS_RegJobs)
                {
                    _supervisorMobilityRepository.DeleteJobObservation(item.JobObservation);
                }
            }
            await _supervisorMobilityRepository.SaveChangesAsync();

            var result = await _supervisorMobilityRepository.DeleteSOSReview(SOS_Entity);

            if (result == 0)
            {
                return NotFound();

            }
            else
            {
                //await _supervisorMobilityRepository.SaveChangesAsync();

                return Ok();
            }


        }//end delete

    }//end main clas
}//end namespace
