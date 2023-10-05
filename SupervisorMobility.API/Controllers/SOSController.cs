using AutoMapper;
using DocumentFormat.OpenXml.Office2010.Drawing;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.SOS_Review;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.AreaDtos;
using SupervisorMobility.API.Models.JobObservationDtos;
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
        
        [HttpGet("Registers/{SOSid}")]
        public async Task<ActionResult<IEnumerable<SOSReviewsRegisterDto>>> SOSReviewRegisters(int SOSid, bool includeCollections = false)
        {

            if (includeCollections)
            {
                var SOSRevierWhitDistributions = await _supervisorMobilityRepository.GetAllSOSReviewsRegisters(SOSid);
                return Ok(_mapper.Map<IEnumerable<SOSReviewsRegisterDto>>(SOSRevierWhitDistributions));

            }
            else
            {
                var SOS_Reviews = await _supervisorMobilityRepository
                                .GetAllSOSReviewsRegisters(SOSid);
                return Ok(_mapper.Map<IEnumerable<SOSReviewsRegisterDto>>(SOS_Reviews));
            }

        }//end get all registers
         //
        
        [HttpGet("Registers/UserOp/{SOSid}")]
        public async Task<ActionResult<IEnumerable<SOSRegUserOperationDto>>> SOSReviewRegistersUserOperation(int SOSid, bool includeCollections = false)
        {

            if (includeCollections)
            {
                var SOSRevierWhitDistributions = await _supervisorMobilityRepository.GetAllSOSRegUserOperations(SOSid);
                return Ok(_mapper.Map<IEnumerable<SOSRegUserOperationDto>>(SOSRevierWhitDistributions));
            }
            else
            {
                var SOS_Reviews = await _supervisorMobilityRepository
                                .GetAllSOSRegUserOperations(SOSid);
                return Ok(_mapper.Map<IEnumerable<SOSRegUserOperationDto>>(SOS_Reviews));
            }

        }//end get all registers

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
            List<User> Users = new List<User>();
            bool haveUsers = false;

            if (SOSentity.Supervisors != null)
            {
                haveUsers = true;
                foreach (var Sub in SOSentity.Supervisors)
                {
                    var usr = await _supervisorMobilityRepository.GetUserAsync(Sub.UserId);
                    if(usr != null)
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

                if (haveUsers)
                {
                    foreach (var item in Users)
                    {
                        _supervisorMobilityRepository.SOSReviewAddUser(finalSOSReview, item);
                    }
                }

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

        [HttpPost("Registers/UserOp/{SOSid}")]
        public async Task<ActionResult<SOSRegUserOperationDto>> CreateSOSRegUserOperation(int SOSid, int SupervisorId, int OperationId)
        {

            SOSRegUserOperation SOSRegUserOp = new();

            if (SOSid == 0)
            {
                return NotFound("Cant Not Createe Whit SOS Review Id 0"); 
            }
            else
            {
                SOSRegUserOp.SOSReviewProgramid = SOSid;

            }
            
            if (OperationId == 0)
            {
                return NotFound("Cant Not Createe Whit Operation Id 0"); 
            }
            else
            {
                SOSRegUserOp.OperationId = OperationId;

            }
            
            if (SupervisorId == 0)
            {
                return NotFound("Cant Not Createe Whit Supervisor Id 0"); 
            }
            else
            {
                SOSRegUserOp.SupervisorId = SupervisorId;
            }



            var result = await _supervisorMobilityRepository.AddSOSRegUserOperation(SOSRegUserOp);


            if (result > 0)
            {
                await _supervisorMobilityRepository.SaveChangesAsync();
        
                return Ok(SOSRegUserOp);
            }
            else
            {
                return NotFound();
            }


        }//end post create register


        [HttpPost("Registers/{SOSid}")]
        public async Task<ActionResult<SOSReviewWithAllDto>> CreateSOSRegister(int SOSid, int month, int year, JobObservationForCreationDto JobEntity)
        {
           
            var finalJob =  _mapper.Map<JobObservation>(JobEntity);

            if(finalJob.OperationId == 0)
            {
                finalJob.OperationId = null;
            }

            if(finalJob.OperatorId == 0)
            {
                finalJob.OperatorId = null;
            }
           
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


        [HttpPut("Registers/UserOp/Register/{SOSRegid}")]
        public async Task<ActionResult> UpdateSosReview(int SOSRegid,
           SOSRegUserOperationForUpdateDto sosUpdateEntity)
        {

            var SOS_Entity = await _supervisorMobilityRepository.GetSOSRegUserOperation(SOSRegid);

            if (SOS_Entity == null)
            {
                return NotFound();
            }

            if(SOS_Entity.SupervisorId != sosUpdateEntity.SupervisorId)
            {
                //todas las jobs se cambia el supervisor

                var Jobs = await _supervisorMobilityRepository.GetAllJobObservationsAsync(false);
           
            }

            var result = await _supervisorMobilityRepository.UpdateRegUserOperation(sosUpdateEntity, SOS_Entity);

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

        [HttpPut("{SOSid}")]
        public async Task<ActionResult> UpdateSosReview(int SOSid,
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
