using AutoMapper;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Drawing;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.SOS_Review;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.AreaDtos;
using SupervisorMobility.API.Models.DistributionDtos;
using SupervisorMobility.API.Models.JobObservationDtos;
using SupervisorMobility.API.Models.SOSReviewDtos;
using SupervisorMobility.API.Services;
using System.Diagnostics;

namespace SupervisorMobility.API.Controllers
{

    [Route("api/SOSReview")]
    [ApiController]

    public class SOSProgramReviewRegisterController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ISupervisorMobilityRepository _supervisorMobilityRepository;
        private readonly IAssyChartService _assyChartService;

        public SOSProgramReviewRegisterController(ISupervisorMobilityRepository supervisorMobilityRepository, IAssyChartService assyChartService,
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

            //if (finalJob.OperationId == 0)
            //{
            //    finalJob.OperationId = null;
            //}

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
            finalSOSReg.OperationId = finalJob.Operations.FirstOrDefault().OperationId;


            var result = await _supervisorMobilityRepository.AddSOSReviewRegister(finalSOSReg);


            if (result > 0)
            {
                await _supervisorMobilityRepository.SaveChangesAsync();

                var createdRegisterToReturn =
                    _mapper.Map<SOSReviewsRegisterDto>(finalSOSReg);

                createdRegisterToReturn.JobObservation = _mapper.Map<JobObservationDto>(finalJob);

                return Ok(createdRegisterToReturn);
            }
            else
            {
                return NotFound();
            }


        }//end post create register

        public class DistSelect
        {
            public DistributionWithNavigationPropertiesDto distribution { get; set; }
            public bool isSelected { get; set; } = false;
        }
        public class RequestMassiveDistributionSos
        {
            public List<DistSelect> distributions { get; set; } = new List<DistSelect>();
            public List<JobObservationForCreationDto> Jobs { get; set; } = new List<JobObservationForCreationDto> { };
        }


        [HttpPost("Registers/{sos_id}/ApplySuggest")]
        public async Task<ActionResult> MassiveCreate(int sos_id, RequestMassiveDistributionSos JobsSuggestData)
        {
            List<JobObservationForCreationDto> JobsSuggest = JobsSuggestData.Jobs;
            List<DistSelect> DistSuggest = JobsSuggestData.distributions;


            var SOS_Review = await _supervisorMobilityRepository.GetSOSasync(sos_id, true, true, true);
            //var sosUpdateEntity = _mapper.Map<SOSReviewForUpdateDto>(SOS_Review);
            var JobRegisterExist = await _supervisorMobilityRepository.GetAllSOSReviewsRegisters(sos_id);
            var UserOpRegistersExist = await _supervisorMobilityRepository.GetAllSOSRegUserOperations(sos_id);


            foreach (var job in JobsSuggest)
            {
                //validar si ya hay un registro para actualizarlo
                Debug.WriteLine($"job {job.StartDate} {job.SupervisorId}");

                int maxRetries = 5; // Número máximo de intentos
                TimeSpan retryInterval = TimeSpan.FromSeconds(1); // Intervalo de tiempo entre intentos (5 segundos en este caso)
                int retries = 0;

                while (retries < maxRetries)
                {
                    try
                    {
                        if (JobRegisterExist.Any(j => j.OperationId == job.Operations.FirstOrDefault().OperationId))
                        {
                            Debug.WriteLine($"Ya exite el registro ");

                            var jobObservationEntity = JobRegisterExist.ToList().Find(j => j.OperationId == job.Operations.FirstOrDefault().OperationId).JobObservation;
                            var ForUpdate = _mapper.Map<JobObservationForUpdateDto>(jobObservationEntity);

                            ForUpdate.SupervisorId = job.SupervisorId;

                            JobObservationVersion HistoryToAdd = await _assyChartService.CreateHistoryJobObservationAsync(jobObservationEntity);

                            //Actualiza la jobobsevation
                            _mapper.Map(ForUpdate, jobObservationEntity);

                            //añadimos la version anterior a la jobOb actualizada
                            if (HistoryToAdd != null)
                            {
                                //optenemos la nueva version
                                var jobtoaddversion = await _supervisorMobilityRepository.GetJobObservationAsync(jobObservationEntity.JobObservationId, true);
                                HistoryToAdd.DateModification = DateTime.Now;
                                HistoryToAdd.resumeVersion = "supervisor";
                                HistoryToAdd.MadeBy = "SOS REView system";
                                //añadimos
                                bool added = await _supervisorMobilityRepository.AddHistoyToJobObservationAsync(HistoryToAdd, jobtoaddversion);

                            }

                            var RegUserOpEntity = UserOpRegistersExist.ToList().Find(r => r.OperationId == job.Operations.FirstOrDefault().OperationId);

                            if (RegUserOpEntity != null)
                            {
                                var RegForUpdate = _mapper.Map<SOSRegUserOperationForUpdateDto>(RegUserOpEntity);
                                if (RegForUpdate.SupervisorId != job.SupervisorId)
                                {
                                    RegForUpdate.SupervisorId = job.SupervisorId;
                                    var resultUpdate = await _supervisorMobilityRepository.UpdateRegUserOperation(RegForUpdate, RegUserOpEntity);
                                }
                            }
                            else
                            {
                                SOSRegUserOperation SOSRegUserOp = new();
                                SOSRegUserOp.SOSReviewProgramid = sos_id;
                                SOSRegUserOp.OperationId = job.Operations.FirstOrDefault().OperationId;
                                SOSRegUserOp.SupervisorId = job.SupervisorId;

                                var CreateRegUserOper = await _supervisorMobilityRepository.AddSOSRegUserOperation(SOSRegUserOp);
                            }

                            var RegJobEntity = JobRegisterExist.ToList().Find(r => r.OperationId == job.Operations.FirstOrDefault().OperationId);

                            if (RegJobEntity != null)
                            {
                                //Update jobregister
                                var RegForUpdate = _mapper.Map<SOSReviewsRegisterForUpdateDto>(RegJobEntity);

                                var resultUpdate = await _supervisorMobilityRepository.UpdateRegisterJobObservation(RegForUpdate, RegJobEntity);
                            }
                            else
                            {

                                SOSRegisterJobObservation finalSOSReg = new();

                                finalSOSReg.SOSReviewProgramid = sos_id;
                                finalSOSReg.Month = job.StartDate.Value.Month;
                                finalSOSReg.Year = job.StartDate.Value.Year;
                                finalSOSReg.JobObservationId = jobObservationEntity.JobObservationId;
                                finalSOSReg.OperationId = jobObservationEntity.Operations.FirstOrDefault().OperationId;

                                var CreateRegJobOper = await _supervisorMobilityRepository.AddSOSReviewRegister(finalSOSReg);
                            }

                        }
                        else
                        {
                            Debug.WriteLine($"No Exite");

                            if (job.JobObservationId != null && job.JobObservationId  != 0 )
                            {

                                var finalJob = await _supervisorMobilityRepository.GetJobObservationAsync((int)job.JobObservationId, true);
                                    
                                _mapper.Map(job, finalJob);

                                finalJob.Operations = new List<Operation>();

                                foreach (var op in job.Operations)
                                {
                                    Operation opAdd = await _supervisorMobilityRepository.GetOperationForDistributionAsync(job.DistributionId, op.OperationId);
                                    finalJob.Operations.Add(opAdd);
                                }

                                SOSRegisterJobObservation finalSOSReg = new();
                                finalSOSReg.SOSReviewProgramid = sos_id;
                                finalSOSReg.Month = job.StartDate.Value.Month;
                                finalSOSReg.Year = job.StartDate.Value.Year;
                                finalSOSReg.JobObservationId = finalJob.JobObservationId;
                                finalSOSReg.OperationId = finalJob.Operations.FirstOrDefault().OperationId;

                                var resultcreate = await _supervisorMobilityRepository.AddSOSReviewRegister(finalSOSReg);


                                SOSRegUserOperation SOSRegUserOp = new();
                                SOSRegUserOp.SOSReviewProgramid = sos_id;
                                SOSRegUserOp.OperationId = job.Operations.FirstOrDefault().OperationId;
                                SOSRegUserOp.SupervisorId = job.SupervisorId;

                                var CreateRegUserOper = await _supervisorMobilityRepository.AddSOSRegUserOperation(SOSRegUserOp);

                            }
                            else
                            {

                                var finalJob = _mapper.Map<JobObservation>(job);

                                //if (finalJob.OperationId == 0)
                                //{
                                //    finalJob.OperationId = null;
                                //}
                                finalJob.PlannedStartDate = finalJob.StartDate;

                                finalJob.Operations = new List<Operation>();

                                foreach (var op in job.Operations)
                                {
                                    Operation opAdd = await _supervisorMobilityRepository.GetOperationForDistributionAsync(job.DistributionId, op.OperationId);
                                    finalJob.Operations.Add(opAdd);
                                }

                                await _supervisorMobilityRepository.AddJobObservation(finalJob);

                                SOSRegisterJobObservation finalSOSReg = new();
                                finalSOSReg.SOSReviewProgramid = sos_id;
                                finalSOSReg.Month = job.StartDate.Value.Month;
                                finalSOSReg.Year = job.StartDate.Value.Year;
                                finalSOSReg.JobObservationId = finalJob.JobObservationId;
                                finalSOSReg.OperationId = finalJob.Operations.FirstOrDefault().OperationId;

                                var resultcreate = await _supervisorMobilityRepository.AddSOSReviewRegister(finalSOSReg);


                                SOSRegUserOperation SOSRegUserOp = new();
                                SOSRegUserOp.SOSReviewProgramid = sos_id;
                                SOSRegUserOp.OperationId = job.Operations.FirstOrDefault().OperationId;
                                SOSRegUserOp.SupervisorId = job.SupervisorId;

                                var CreateRegUserOper = await _supervisorMobilityRepository.AddSOSRegUserOperation(SOSRegUserOp);

                            }

                        }


                        Console.WriteLine($"Intento {retries + 1} ");

                        // Si la operación tiene éxito, puedes salir del bucle
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"falló job {job.Operations.FirstOrDefault().OperationId}: {ex.Message}");

                        // Incrementa el número de intentos

                        // Espera el intervalo de tiempo antes de volver a intentarlo
                        await Task.Delay(retryInterval);
                    }

                }


                //busca si el supervisor existe dentro de los usuarios con acceso a la sos review
                if (!SOS_Review.Supervisors.Any(u => u.UserId == job.SupervisorId))
                {
                    //si no existe lo añade, para que el usuario pueda ver la sos desde el client side
                    var usr = await _supervisorMobilityRepository.GetUserAsync(job.SupervisorId);
                    _supervisorMobilityRepository.SOSReviewAddUser(SOS_Review, usr);
                }

            }

            //Actualiza las distribuciones a las cuales ya se les aplico la sugerencia, para evitar que vuelvan a
            //crear una repetida
            foreach (var sugg in DistSuggest)
            {
                if (sugg.isSelected)
                {
                    var SuggestDistribution = await _supervisorMobilityRepository.GetDistSuggestion(sos_id, sugg.distribution.DistributionId);
                    SuggestDistribution.SuggestionApplied = true;
                }
            }


            await _supervisorMobilityRepository.SaveChangesAsync();


            return Ok();
        }//end masive Suggest

    }//end main clas
}//end namespace
