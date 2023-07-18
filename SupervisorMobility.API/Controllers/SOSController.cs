using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.DataAccess.Entities.SOS_Review;
using SupervisorMobility.API.Models.AreaDtos;
using SupervisorMobility.API.Models.SOSReviewDtos;
using SupervisorMobility.API.Services;

namespace SupervisorMobility.API.Controllers
{

    [Route("api/SOSReview")]
    [ApiController]

    public class SOSController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ISupervisorMobilityRepository _supervisorMobilityRepository;

        public SOSController(ISupervisorMobilityRepository supervisorMobilityRepository,
            IMapper mapper)
        {
            _supervisorMobilityRepository = supervisorMobilityRepository ??
                throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SOSReviewWithAllDto>>> GetAllSos(bool includeCollections = false)
        {

            if (includeCollections)
            {
                var SOSRevierWhitDistributions = await _supervisorMobilityRepository.GetAllSOSReviews();
                return Ok(_mapper.Map<IEnumerable<SOSReviewWithAllDto>>(SOSRevierWhitDistributions));

            }
            else
            {
                var SOS_Reviews = await _supervisorMobilityRepository
                                .GetAllSOSReviews();
                return Ok(_mapper.Map<IEnumerable<SOSReviewWithOutDataDto>>(SOS_Reviews));

            }

        }//end get all

        [HttpGet("{sosId}", Name = "GetSOS")]
        public async Task<ActionResult<SOSReviewWithAllDto>> GetSOS(int sosId, bool includeCollections = false)
        {

            var SOS_Review = await _supervisorMobilityRepository.GetSOSasync(sosId);

            if (SOS_Review == null)
            {
                return NotFound();
            }

            if (includeCollections)
            {
                return Ok(_mapper.Map<SOSReviewWithAllDto>(SOS_Review));
            }
            return Ok(_mapper.Map<SOSReviewWithOutDataDto>(SOS_Review));
        }//end get one

        [HttpPost]
        public async Task<ActionResult<SOSReviewWithAllDto>> CreateSOSReview(
           SOSReviewForCreateDto SOSentity)
        {

            var finalSOSReview = _mapper.Map<SOSReviewProgram>(SOSentity);


            var result = await _supervisorMobilityRepository.AddSOSReview(finalSOSReview);
        

            if (result > 0)
            {
                await _supervisorMobilityRepository.SaveChangesAsync();

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
        public async Task<ActionResult> UpdateArea(int SOSid,
            SOSReviewForUpdateDto sosUpdateEntity)
        {

            var SOS_Entity = await _supervisorMobilityRepository
                .GetSOSasync(SOSid);

            if (SOS_Entity == null)
            {
                return NotFound();
            }

            var result = await _supervisorMobilityRepository.UpdateSOSReview(sosUpdateEntity, SOS_Entity);

            if(result == 0)
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
              .GetSOSasync(SOSid);

            if (SOS_Entity == null)
            {
                return NotFound();
            }

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
