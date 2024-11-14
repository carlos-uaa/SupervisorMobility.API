using AutoMapper;
using DocumentFormat.OpenXml.Bibliography;
using FuzzyString;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Logging.Abstractions;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.ADUser;
using SupervisorMobility.API.Models.ChecklistAnswerDtos;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.JobObservationDtos;
using SupervisorMobility.API.Models.JobPaginationDtos;
using SupervisorMobility.API.Models.NotificationDtos;
using SupervisorMobility.API.Services;

namespace SupervisorMobility.API.Controllers
{
    [EnableCors("Cors")]
    [Route("api/jobobservations")]
    [ApiController]
    public class JobObservationController : ControllerBase
    {
        private readonly ISupervisorMobilityRepository _supervisorMobilityRepository;
        readonly IAssyChartService _assyChartService;
        private readonly IMapper _mapper;
        private readonly IEmailService _email;
        private readonly IWebHostEnvironment _env;

        public JobObservationController(ISupervisorMobilityRepository supervisorMobilityRepository, IMapper mapper, IWebHostEnvironment env,
            IAssyChartService assyChartService, IEmailService emailService)
        {
            _email = emailService;
            _supervisorMobilityRepository = supervisorMobilityRepository ??
                throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
            _assyChartService = assyChartService;
            _env = env;

        }

        [HttpPost]
        public async Task<ActionResult<JobObservationWithoutNavigationPropertiesDto>> CreateJobObservation(
            JobObservationForCreationDto jobObservation)
        {
            if (!await _supervisorMobilityRepository.PlantExistAsync(jobObservation.PlantId))
            {
                return NotFound();
            }

            if (!await _supervisorMobilityRepository.AreaExistAsync(jobObservation.AreaId))
            {
                return NotFound("No Area");
            }

            if (!await _supervisorMobilityRepository.DistributionExistsAsync(jobObservation.DistributionId))
            {
                return NotFound("No Distribution");
            }

            //if (!await _supervisorMobilityRepository.OperationExistsAsync(jobObservation.OperationId))
            //{
            //    return NotFound("No Operation");
            //}

            var finalJobObservation = _mapper.Map<JobObservation>(jobObservation);

            //En caso de que no funcione tengo que crear funcion que busque las operaciones y las añada a la lista

            if (finalJobObservation.OperatorId == 0)
            {
                finalJobObservation.OperatorId = null;
            }
            _supervisorMobilityRepository.AddJobObservation(finalJobObservation);
            await _supervisorMobilityRepository.SaveChangesAsync();
            return Ok(finalJobObservation);
        }


        [HttpPost("WithLup")]
        public async Task<ActionResult<JobObservationWithoutNavigationPropertiesDto>> CreateJobObservationWithLup(
            JobObservationWithLupForCreationDto jobObservationAndLup)
        {
            if (!await _supervisorMobilityRepository.PlantExistAsync(jobObservationAndLup.PlantId))
            {
                return NotFound();
            }

            if (!await _supervisorMobilityRepository.AreaExistAsync(jobObservationAndLup.AreaId))
            {
                return NotFound("No Area");
            }

            if (!await _supervisorMobilityRepository.DistributionExistsAsync(jobObservationAndLup.DistributionId))
            {
                return NotFound("No Distribution");
            }

            //if (!await _supervisorMobilityRepository.OperationExistsAsync(jobObservationAndLup.OperationId))
            //{
            //    return NotFound("No Operation");
            //}

            var finalJobObservation = _mapper.Map<JobObservation>(jobObservationAndLup);

            finalJobObservation.Operations = new List<Operation>();

            foreach (var op in jobObservationAndLup.Operations)
            {
                Operation opAdd = await _supervisorMobilityRepository.GetOperationForDistributionAsync(jobObservationAndLup.DistributionId, op.OperationId);
                finalJobObservation.Operations.Add(opAdd);
            }
            //if (finalJobObservation.OperationId == 0)
            //{
            //    finalJobObservation.OperationId = null;
            //}

            if (finalJobObservation.OperatorId == 0)
            {
                finalJobObservation.OperatorId = null;
            }
            _supervisorMobilityRepository.AddJobObservation(finalJobObservation);

            await _supervisorMobilityRepository.SaveChangesAsync();

            return Ok(finalJobObservation);
        }


        public class OperatorSignatureContent
        {
            public IFormFile? File { get; set; }
            public string? JobObservationId { get; set; }
            public FileUpload? Evidence { get; set; }
        }

        [HttpPost("operatorSignature")]
        public async Task<ActionResult<JobObservationWithoutNavigationPropertiesDto>> CreateOperatorSignature([FromForm] OperatorSignatureContent OperatorSignatureContent)
        {

            int jobObservationId = int.Parse(OperatorSignatureContent.JobObservationId);
            var finalJobObservation = await _supervisorMobilityRepository.GetJobObservationAsync(jobObservationId);

            if (OperatorSignatureContent.File != null)
            {
                var file = OperatorSignatureContent.File;
                var uploadResult = new FileUploadForCreationDto();
                string trustedFileNameForStorage = string.Empty;
                var unstrustedFileName = file.FileName;

                trustedFileNameForStorage = Path.GetRandomFileName();
                var path = Path.Combine(_env.ContentRootPath, "uploads\\operatorSignature", trustedFileNameForStorage);

                await using FileStream fs = new(path, FileMode.Create);
                await file.CopyToAsync(fs);

                uploadResult.FileName = unstrustedFileName;
                uploadResult.StorageFileName = trustedFileNameForStorage;
                uploadResult.ContentType = file.ContentType;
                uploadResult.UploadDate = DateTime.Now;

                var fileToReturn = await _assyChartService.CreateFileAsync(uploadResult);
                await _supervisorMobilityRepository.AddOperatorSignatureForJobObservationAsync(finalJobObservation.JobObservationId, fileToReturn);


                await _supervisorMobilityRepository.SaveChangesAsync();
            }

            return Ok(finalJobObservation);
        }


        [HttpGet("filters")]
        public async Task<ActionResult<JOPaginationDto>> GetJobObservationsByFilters(
            DateTime startDate,
            DateTime endDate,
            int jobObsId,
            int plantId,
            int areaId,
            int distributionId,
            int operationId,
            int supervisorId,
            int status,
            int userId,
            int typeId,
            string? searchString,
            int page, int entries, int? sortO, string? sortL)
        {

            var allJobObservations = await _supervisorMobilityRepository.GetJobObservationsByFiltersAsync(startDate, endDate, jobObsId, plantId, areaId, distributionId, operationId, supervisorId, status, userId, typeId, searchString, page, entries, sortO, sortL);
            
            return Ok(allJobObservations);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobObservationDto>>> GetAllJobObservationsAsync(bool includeTree = false, bool includePeople = false,
            bool includeLup = false, bool includeHistory = false, bool includeCkAnswers = false, int idPlant = 0, int idArea = 0, bool ForSosProgram = false,
            int year = 0, int month = 0, int SOSAnualId = 0, int idUser = 0)
        {

            var allJobObservations = await _supervisorMobilityRepository.GetAllJobObservationsAsync(includeTree, includePeople, includeLup, includeHistory, includeCkAnswers, idPlant, idArea, ForSosProgram, year, month, SOSAnualId, idUser);

            if (allJobObservations == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<IEnumerable<JobObservationDto>>(allJobObservations));
        }

        [HttpGet("{jobObservationId}/history")]
        public async Task<ActionResult<IEnumerable<JobObservationHistoryDto>>> GetHistoryJobObservationsAsync(int jobObservationId)
        {
            var allHistory = await _supervisorMobilityRepository.GetAllHistoryJobObservationAsync(jobObservationId);

            return Ok(_mapper.Map<IEnumerable<JobObservationHistoryDto>>(allHistory));
        }

        [HttpGet("{jobObservationId}/history/{historyId}/detail")]
        public async Task<ActionResult<JobObservationHistoryDto>> GetHistoryDetails(int jobObservationId, int historyId)
        {
            var history = await _supervisorMobilityRepository.GetHistoryJobObservationAsync(historyId);

            return Ok(_mapper.Map<JobObservationHistoryDto>(history));
        }

        [HttpDelete("{jobObservationId}/history/{HistoryId}/remove")]
        public async Task<ActionResult> DeleteHistoryJobObservation(int jobObservationId, int HistoryId)
        {
            var jobObservation = await _supervisorMobilityRepository.GetJobObservationAsync(jobObservationId, true);

            if (jobObservation == null)
            {
                return NotFound();
            }


            var HistoryToRemove = await _supervisorMobilityRepository.GetHistoryJobObservationAsync(HistoryId);

            var result = await _supervisorMobilityRepository.DeleteHistoyFromJobObservationAsync(HistoryToRemove, jobObservation);
            await _supervisorMobilityRepository.SaveChangesAsync();

            if (result)
            {
                return Ok();
            }


            return NotFound("Job Observation Version not remove");
        }


        //[EnableCors("Cors")]
        [HttpGet("{jobObservationId}", Name = "GetJobObservation")]
        public async Task<IActionResult> GetJobObservation(int jobObservationId, bool includeTree = false, bool includePeople = false, bool includeLup = false, bool includeHistory = false, bool includeCkAnswers = false)
        {

            //Find Job Observation type
            var jobObservation = await _supervisorMobilityRepository.GetJobObservationAsync(jobObservationId, includeTree, includePeople, includeLup, includeHistory, includeCkAnswers);
            if (jobObservation == null)
            {
                return NotFound();
            }


            return Ok(_mapper.Map<JobObservationDto>(jobObservation));
        }



        //[EnableCors("Cors")]
        [HttpPut("{jobObservationId}")]
        public async Task<ActionResult> UpdateJobObservation(int jobObservationId, RequestJobObservationADuser request)
        {

            JobObservationForUpdateDto jobObservationForUpdate = request.JobObservation;
            string auser = request.LoggedUser;

            if (!await _supervisorMobilityRepository.PlantExistAsync(jobObservationForUpdate.PlantId))
            {
                return NotFound();
            }

            if (!await _supervisorMobilityRepository.AreaExistAsync(jobObservationForUpdate.AreaId))
            {
                return NotFound("No Area");
            }

            if (!await _supervisorMobilityRepository.DistributionExistsAsync(jobObservationForUpdate.DistributionId))
            {
                return NotFound("No Distribution");
            }


            var jobObservationEntity = await _supervisorMobilityRepository.GetJobObservationAsync(jobObservationId, includeOperations: true);
            if (jobObservationEntity == null)
            {
                return NotFound("Job Observation Not Found");
            }

            if (jobObservationForUpdate.Status == 6 && jobObservationEntity.Status != jobObservationForUpdate.Status && auser != "S.M. System")
            {
                //crear notificacion
                NotificationToCreateDto newnotify = new NotificationToCreateDto();
                newnotify.MadeBy = auser;
                newnotify.UserId = jobObservationForUpdate.SupervisorId;
                newnotify.IsAccepted = true;
                newnotify.IsActive = true;
                newnotify.NotificationText = $"The JobObservation with id: {jobObservationEntity.JobObservationId} was terminated by the user {auser}";
                newnotify.NotificationType = "FinishJobObservation";

                var notadd = await _assyChartService.CreateNotificationAsync(newnotify);
                //if (notadd != null)
                //{
                //    var emailMessage = _email.CreateEmailMessage(auser, "Este es un mensaje de prueba enviado desde job observation");
                //    _email.Send(emailMessage);
                //}


                //Si la Job Actual tiene un  sos plan id puedo buscar una del siguiente año directamente
                //if (jobObservationEntity.PlantId != null && jobObservationEntity.AreaId != null && jobObservationEntity.SupervisorId != null)
                //{
                //    var SOS_Review_NextYear = await _supervisorMobilityRepository.FindSOSSupervisor((int)jobObservationEntity.PlantId, (int)jobObservationEntity.AreaId, jobObservationEntity.FinishedDate.Value.Year + 1,  (int) jobObservationEntity.SupervisorId);

                //    //Si existe un plan del siguiente año, tengo que buscar la job y actualizar sus datos
                //    if(SOS_Review_NextYear != null)
                //    {
                //        //buscar la job del siguiente año y actualizar la job
                //    }
                //    else
                //    {
                //        //crear un plan para el suigueinte año y añadir la nueva job 
                //    }
                //}

                //Crear la job del typo 5 siempre y cuando sea del sos program, ya que revisa la operacion

                if (jobObservationEntity.PlantId != null && jobObservationEntity.AreaId != null && jobObservationEntity.DistributionId != null && jobObservationEntity.Operations.FirstOrDefault() != null)
                {

                   
                    List<JobObservation>? nextYearJobs = await _supervisorMobilityRepository.FindNextYearJobObservations( 
                        (int)jobObservationEntity.PlantId,
                        (int)jobObservationEntity.AreaId,
                        (int)jobObservationEntity.DistributionId,
                        jobObservationEntity.Operations,
                        (int)jobObservationEntity.SupervisorId,
                        jobObservationForUpdate.FinishedDate.Value.Year + 1);

                    IEnumerable<JobCategoryStructure> _checklistCategories = await _supervisorMobilityRepository.GetChecklistCategoriesAsync(false);
                    
                    string jobCategoryStructureIds = "";
                    foreach (var category in _checklistCategories)
                    {
                        jobCategoryStructureIds += category.JobCategoryStructureId + "|";
                    }

                    if (nextYearJobs == null || nextYearJobs.Count == 0)
                    {

                        //no existe hay que crearla
                        JobObservation newYearJob = new JobObservation();

                        newYearJob.Type = 5;

                        newYearJob.PlantId = jobObservationEntity.PlantId;
                        newYearJob.AreaId = jobObservationEntity.AreaId;
                        newYearJob.DistributionId = jobObservationEntity.DistributionId;

                     
                        foreach (var op in jobObservationEntity.Operations)
                        {
                            if (!newYearJob.Operations.Any(existingOp => existingOp.OperationId == op.OperationId))
                            {
                                newYearJob.Operations.Add(op); // Agregar la operación faltante si no está en consolidatedFutureJob
                            }
                        }

                        newYearJob.SupervisorId = jobObservationEntity.SupervisorId;

                        newYearJob.StartDate = jobObservationEntity.FinishedDate?.AddYears(1) ?? DateTime.Now.AddYears(1);
                        newYearJob.PlannedStartDate = jobObservationEntity.FinishedDate?.AddYears(1) ?? DateTime.Now.AddYears(1);

                        newYearJob.EndDate = newYearJob.PlannedStartDate;

                        newYearJob.SectionIds = jobCategoryStructureIds;


                        var res = await _supervisorMobilityRepository.AddJobObservation(newYearJob);

                        if (res > 0)
                        {
                            Distribution distribution = await _supervisorMobilityRepository.GetDistributionOnlyIdAsync((int)jobObservationEntity.DistributionId, false);

                            NotificationToCreateDto notifynextYear = new NotificationToCreateDto();
                            notifynextYear.MadeBy = auser;
                            notifynextYear.UserId = jobObservationForUpdate.SupervisorId;
                            notifynextYear.IsAccepted = true;
                            notifynextYear.IsActive = true;
                            notifynextYear.NotificationType = $"SOS Anual - New Job Observation";
                            notifynextYear.NotificationText = "Estimado Supervisor,\\n\\nHemos detectado una Job Observation." +
                                " A continuación, te informamos sobre las acciones que se tomarán en función del estado del SOS Anual para el próximo año " +
                                "El sistema procederá a crear una nueva entrada en el SOS Anual con la información proporcionada por la Job Observation.:" +
                                $"\\n\\n Distribucion: {distribution?.Description} - {distribution?.Code}" +
                                 $"\\n Fecha {newYearJob.StartDate}" +
                                "\\n\\nPor favor, asegúrate de que la información esté actualizada para evitar posibles inconsistencias.\r\n\r\nSaludos cordiales,\r\n[SupervisorMobility]";


                            var notynextYear = await _assyChartService.CreateNotificationAsync(notifynextYear);
                            await _supervisorMobilityRepository.SaveChangesAsync();

                        }
                    }
                    else
                    {
                        // Crear una nueva JobObservation que consolidará las operaciones deseadas

                        var consolidatedFutureJob = new JobObservation
                        {
                            Type = 5,
                            PlantId = jobObservationEntity.PlantId,
                            AreaId = jobObservationEntity.AreaId,
                            DistributionId = jobObservationEntity.DistributionId,
                            Operations = new List<Operation>(),

                            SupervisorId = jobObservationEntity.SupervisorId,

                            StartDate = jobObservationForUpdate.FinishedDate.Value.AddYears(1),
                            PlannedStartDate = jobObservationForUpdate.FinishedDate.Value.AddYears(1),
                            EndDate = jobObservationForUpdate.FinishedDate.Value.AddYears(1),
                            SectionIds = jobCategoryStructureIds

                        };


                        // Iterar sobre las jobs futuras y consolidar las operaciones necesarias
                        foreach (var futureJob in nextYearJobs)
                        {
                            // Iterar sobre cada operación de la futureJob
                            foreach (var op in futureJob.Operations.ToList()) // `ToList` evita la modificación de la colección mientras iteramos
                            {
                                // Si la operación existe en `jobObservationEntity`, la movemos a `consolidatedFutureJob`
                                if (jobObservationEntity.Operations.Any(currentOp => currentOp.OperationId == op.OperationId))
                                {
                                    Operation opAdd = await _supervisorMobilityRepository.GetOperationForDistributionAsync((int)jobObservationEntity.DistributionId, (int)op.OperationId);
                                    consolidatedFutureJob.Operations.Add(opAdd);

                                    futureJob.Operations.Remove(op); // Remover de la futureJob después de consolidarla
                                }
                            }

                            // Eliminar futureJob si se queda sin operaciones
                            if (!futureJob.Operations.Any())
                            {
                                _supervisorMobilityRepository.PermanentDeleteJobObservation(futureJob);
                                //_context.JobObservations.Remove(futureJob);
                            }
                        }


                        // Validar que `consolidatedFutureJob` contiene todas las operaciones de `jobObservationEntity`
                        foreach (var op in jobObservationEntity.Operations)
                        {
                            if (!consolidatedFutureJob.Operations.Any(existingOp => existingOp.OperationId == op.OperationId))
                            {
                                consolidatedFutureJob.Operations.Add(op); // Agregar la operación faltante si no está en consolidatedFutureJob
                            }
                        }


                        var res = await _supervisorMobilityRepository.AddJobObservation(consolidatedFutureJob);

                        if (res > 0)
                        {

                            Distribution distribution = await _supervisorMobilityRepository.GetDistributionOnlyIdAsync((int)jobObservationEntity.DistributionId, false);

                            DateTime FechaActual = jobObservationForUpdate.FinishedDate.Value.AddYears(1);


                            NotificationToCreateDto NotifyUpdateNextYear = new NotificationToCreateDto();
                            NotifyUpdateNextYear.MadeBy = auser;
                            NotifyUpdateNextYear.UserId = jobObservationForUpdate.SupervisorId;
                            NotifyUpdateNextYear.IsAccepted = true;
                            NotifyUpdateNextYear.IsActive = true;
                            NotifyUpdateNextYear.NotificationType = $"Actualizacion del SOS Anual - Update Job Observation";
                            NotifyUpdateNextYear.NotificationText = $"Estimado Supervisor,\\n\\nHemos detectado una Job Observation." +
                                " A continuación, te informamos sobre las acciones que se tomarán en función del estado del SOS Anual para el próximo año: " +
                                $"\\n Distribucion: {distribution?.Description} - {distribution?.Code}" +
                                $"\\n{consolidatedFutureJob.StartDate} → {FechaActual}" +
                                "\\n\\nLos datos relacionados serán actualizados automáticamente con la nueva información." +
                                "\\n\\nPor favor, asegúrate de que la información esté actualizada para evitar posibles inconsistencias.\r\n\r\nSaludos cordiales,\r\n[SupervisorMobility]";

                            var notynextYearUpdate = await _assyChartService.CreateNotificationAsync(NotifyUpdateNextYear);
                            consolidatedFutureJob.StartDate = FechaActual;

                            await _supervisorMobilityRepository.SaveChangesAsync();
                        }


                    }





                }

            }


            //Crear copia de la version anterior
            JobObservationVersion HistoryToAdd = await _assyChartService.CreateHistoryJobObservationAsync(jobObservationEntity);

            //Creamos mensaje de cambios
            string resumeChanges = "";

            if (jobObservationEntity.IsActive != jobObservationForUpdate.IsActive)
            {
                resumeChanges += "IsActive, ";
            }
            if (jobObservationEntity.PlantId != jobObservationForUpdate.PlantId)
            {
                resumeChanges += "plant, ";
            }
            if (jobObservationEntity.AreaId != jobObservationForUpdate.AreaId)
            {
                resumeChanges += "area, ";
            }
            if (jobObservationEntity.DistributionId != jobObservationForUpdate.DistributionId)
            {
                resumeChanges += "distribution, ";
            }
            if (jobObservationEntity.Operations != jobObservationForUpdate.Operations)
            {
                resumeChanges += "operation, ";
            }
            if (jobObservationEntity.SupervisorId != jobObservationForUpdate.SupervisorId)
            {
                resumeChanges += "supervisor, ";
            }
            if (jobObservationEntity.OperatorId != jobObservationForUpdate.OperatorId)
            {
                resumeChanges += "operator, ";
            }
            if (jobObservationEntity.Type != jobObservationForUpdate.Type)
            {
                resumeChanges += "Type, ";
            }
            if (jobObservationEntity.StartDate != jobObservationForUpdate.StartDate)
            {
                resumeChanges += "date Start, ";
            }
            if (jobObservationEntity.EndDate != jobObservationForUpdate.EndDate)
            {
                resumeChanges += "date End, ";
            }
            if (jobObservationEntity.FinishedDate != jobObservationForUpdate.FinishedDate)
            {
                resumeChanges += "Date Finalized, ";
            }
            if (jobObservationEntity.Justification != jobObservationForUpdate.Justification)
            {
                resumeChanges += "Justification , ";
            }
            if (jobObservationEntity.Status != jobObservationForUpdate.Status)
            {
                resumeChanges += "Status, ";
            }
            if (jobObservationEntity.Option != jobObservationForUpdate.Option)
            {
                resumeChanges += "Option, ";
            }
            if (jobObservationEntity.Anomaly != jobObservationForUpdate.Anomaly)
            {
                resumeChanges += "Anomaly, ";
            }
            if (jobObservationEntity.HOEStandardTimes != jobObservationForUpdate.HOEStandardTimes)
            {
                resumeChanges += "HOEStandardTimes, ";
            }
            if (jobObservationEntity.ModelsSpecification != jobObservationForUpdate.ModelsSpecification)
            {
                resumeChanges += "Models, ";
            }
            if (jobObservationEntity.Cycles != jobObservationForUpdate.Cycles)
            {
                resumeChanges += "Cicles, ";
            }
            if (jobObservationEntity.SsvCommentary != jobObservationForUpdate.SsvCommentary)
            {
                resumeChanges += "SsvCommentary, ";
            }
            if (jobObservationEntity.OperatorCommentary != jobObservationForUpdate.OperatorCommentary)
            {
                resumeChanges += "OperatorCommentary, ";
            }
            if (jobObservationEntity.SsvSignature != jobObservationForUpdate.SsvSignature)
            {
                resumeChanges += "SsvSignature, ";
            }
            if (jobObservationEntity.OperatorSignature != jobObservationForUpdate.OperatorSignature)
            {
                resumeChanges += "OperatorSignature, ";
            }
            if (jobObservationEntity.ReleasedFeedback != jobObservationForUpdate.ReleasedFeedback)
            {
                resumeChanges += "ReleasedFeedback , ";
            }
            if (jobObservationEntity.KpiId != jobObservationForUpdate.KpiId)
            {
                resumeChanges += "KPI, ";
            }
            if (jobObservationEntity.TaktTime != jobObservationForUpdate.TaktTime)
            {
                resumeChanges += "Takt Time, ";
            }
            if (jobObservationEntity.Questions != jobObservationForUpdate.Questions)
            {
                resumeChanges += "Questions , ";
            }

            // Remove the trailing comma and space
            if (resumeChanges.EndsWith(", "))
            {
                resumeChanges = resumeChanges.Substring(0, resumeChanges.Length - 2);
            }

            //if (jobObservationForUpdate.OperationId == 0)
            //{
            //    jobObservationForUpdate.OperationId = null;
            //}
            if (jobObservationForUpdate.OperatorId == 0)
            {
                jobObservationForUpdate.OperatorId = null;
            }



            //Actualiza la jobobsevation
            _mapper.Map(jobObservationForUpdate, jobObservationEntity);

            jobObservationEntity.Operations = new List<Operation>();

            foreach (var op in jobObservationForUpdate.Operations)
            {
                Operation opAdd = await _supervisorMobilityRepository.GetOperationForDistributionAsync(jobObservationForUpdate.DistributionId, (int)op.OperationId);
                jobObservationEntity.Operations.Add(opAdd);
            }

            //añadimos la version anterior a la jobOb actualizada
            if (HistoryToAdd != null)
            {
                //optenemos la nueva version
                var jobtoaddversion = await _supervisorMobilityRepository.GetJobObservationAsync(jobObservationId, true);
                HistoryToAdd.DateModification = DateTime.Now;
                HistoryToAdd.resumeVersion = resumeChanges;
                HistoryToAdd.MadeBy = auser;
                //añadimos
                bool added = await _supervisorMobilityRepository.AddHistoyToJobObservationAsync(HistoryToAdd, jobtoaddversion);
                //add to 

                if (!added)
                {
                    return NotFound("Fail updating history");
                }

            }



            await _supervisorMobilityRepository.SaveChangesAsync();

            return Ok();

        }


        [HttpGet("NextYear")]
        public async Task<ActionResult<IEnumerable<JobObservationDto>>> GetAllNextYearJobsObservations(int plantId, int areaId, int year)
        {

            var allJobObservations = await _supervisorMobilityRepository.GetAllNextYearJobsObservations(plantId, areaId, year);

            if (allJobObservations == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<IEnumerable<JobObservationDto>>(allJobObservations));
        }



        [HttpDelete("{jobObservationId}")]
        public async Task<ActionResult> DeleteJobObservation(int jobObservationId)
        {
            var jobObservation = await _supervisorMobilityRepository.GetJobObservationAsync(jobObservationId, false);

            if (jobObservation == null)
            {
                return NotFound();
            }

            _supervisorMobilityRepository.DeleteJobObservation(jobObservation);
            await _supervisorMobilityRepository.SaveChangesAsync();

            return Ok();
        }


    }
}
