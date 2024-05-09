using AutoMapper;
using DocumentFormat.OpenXml.Office2010.Drawing;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
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

    public class SOSRegUserOperationController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ISupervisorMobilityRepository _supervisorMobilityRepository;
        private readonly IAssyChartService _assyChartService;

        public SOSRegUserOperationController(ISupervisorMobilityRepository supervisorMobilityRepository, IAssyChartService assyChartService,
            IMapper mapper)
        {
            _supervisorMobilityRepository = supervisorMobilityRepository ??
                throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _assyChartService = assyChartService ??
                throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

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
            
            var SOS_Review = await _supervisorMobilityRepository.GetSOSasync((int)SOSRegUserOp.SOSReviewProgramid, true, true, true);

            if (!SOS_Review.Supervisors.Any(u => u.UserId == SOSRegUserOp.SupervisorId))
            {
                var usr = await _supervisorMobilityRepository.GetUserAsync((int)SOSRegUserOp.SupervisorId);
                _supervisorMobilityRepository.SOSReviewAddUser(SOS_Review, usr);
            }


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
 

        [HttpPut("{sosId}/Registers/UserOp/{SOSRegid}/ByOption/{option}")]
        public async Task<ActionResult> UpdateAllSosReviewRegisterByOption(int sosId, int SOSRegid, int option,
         SOSRegUserOperationForUpdateDto sosUpdateEntity)
        {

            var SOS_Entity = await _supervisorMobilityRepository.GetSOSRegUserOperation(SOSRegid);

            if (SOS_Entity == null)
            {
                return NotFound();
            }

            var Jobs = await _supervisorMobilityRepository.GetAllSOSReviewsRegisters(sosId);
            var AllSosRegisters = await _supervisorMobilityRepository.GetAllSOSRegUserOperations(sosId);
            var SOS_Review = await _supervisorMobilityRepository.GetSOSasync((int)SOS_Entity.SOSReviewProgramid, true, true, true);


            switch (option)
            {
                case 1:
                    var sample = Jobs.FirstOrDefault(J => J.OperationId == sosUpdateEntity.OperationId);

                    var Operations = await _supervisorMobilityRepository.GetOperationsForDistributionAsync((int)sample.JobObservation.DistributionId);

                    //Todos los reguistros en la misma distribucion solamente
                    AllSosRegisters = AllSosRegisters.Where(r => r.SupervisorId == SOS_Entity.SupervisorId && Operations.Any(o => o.OperationId == r.OperationId)).ToList();

                    //Todas las Jobs en la misma Distribucion solamente
                    Jobs = Jobs.Where(j => j.JobObservation.DistributionId == sample.JobObservation.DistributionId).ToList();

                    break;
                case 2:
                    AllSosRegisters = AllSosRegisters.Where(r => r.SupervisorId == SOS_Entity.SupervisorId).ToList();

                    //Todas las Jobs en el mismo SosReview
                    Jobs = Jobs.Where(j => j.JobObservation.SupervisorId == SOS_Entity.SupervisorId).ToList();

                    //aqui va la eliminacion de usuario pasado
                    var Oldusr = await _supervisorMobilityRepository.GetUserAsync((int)SOS_Entity.SupervisorId);

                    _supervisorMobilityRepository.SOSReviewRemoveUser(SOS_Review, Oldusr);

                    break;

                case 3:
                    //por renglon
                  
                    IEnumerable<SOSRegUserOperation> nuevaEnumerable = Enumerable.Repeat(SOS_Entity, 1);

                    AllSosRegisters = nuevaEnumerable;
                    Jobs = Jobs.Where(j => j.OperationId == sosUpdateEntity.OperationId).ToList();
                    break;
            }

            //Añade el supervisor a los participantes si no existe
            if (!SOS_Review.Supervisors.Any(u => u.UserId == sosUpdateEntity.SupervisorId))
            {
                var usr = await _supervisorMobilityRepository.GetUserAsync((int)sosUpdateEntity.SupervisorId);
                _supervisorMobilityRepository.SOSReviewAddUser(SOS_Review, usr);
            }

            foreach (var reg in AllSosRegisters)
            {
                var RegEntity = await _supervisorMobilityRepository.GetSOSRegUserOperation(reg.SOSRegUserOperationId);
                var RegForUpdate = _mapper.Map<SOSRegUserOperationForUpdateDto>(RegEntity);

                RegForUpdate.SupervisorId = (int)sosUpdateEntity.SupervisorId;

                var result = await _supervisorMobilityRepository.UpdateRegUserOperation(RegForUpdate, RegEntity);

                if (result == 0)
                {
                    Debug.WriteLine($"Error UpdateAllSosReviewRegisterByOption RegId: {reg.SOSRegUserOperationId} ");
                }
                else
                {
                    Debug.WriteLine($"Update RegId: {reg.SOSRegUserOperationId} ");
                }
            }
                    await _supervisorMobilityRepository.SaveChangesAsync();

            foreach (var job in Jobs)
            {
                var jobObservationEntity = await _supervisorMobilityRepository.GetJobObservationAsync((int)job.JobObservationId, false);
                var ForUpdate = _mapper.Map<JobObservationForUpdateDto>(jobObservationEntity);

                ForUpdate.SupervisorId = (int)sosUpdateEntity.SupervisorId;

                JobObservationVersion HistoryToAdd = await _assyChartService.CreateHistoryJobObservationAsync(jobObservationEntity);

                //Actualiza la jobobsevation
                _mapper.Map(ForUpdate, jobObservationEntity);

                //añadimos la version anterior a la jobOb actualizada
                if (HistoryToAdd != null)
                {
                    //optenemos la nueva version
                    var jobtoaddversion = await _supervisorMobilityRepository.GetJobObservationAsync((int)job.JobObservationId, true);
                    HistoryToAdd.DateModification = DateTime.Now;
                    HistoryToAdd.resumeVersion = "supervisor";
                    HistoryToAdd.MadeBy = "SOS REView system";
                    //añadimos
                    bool added = await _supervisorMobilityRepository.AddHistoyToJobObservationAsync(HistoryToAdd, jobtoaddversion);
                    //add to 

                    if (!added)
                    {
                        return NotFound("Fail updating history");
                    }

                }

            }

            await _supervisorMobilityRepository.SaveChangesAsync();

           

            return Ok(SOS_Entity);

        }//end Update 

       

    }//end main clas
}//end namespace
