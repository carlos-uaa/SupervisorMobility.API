using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.ADUser;
using SupervisorMobility.API.Models.JobObservationDtos;
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

        public JobObservationController(ISupervisorMobilityRepository supervisorMobilityRepository, IMapper mapper,
            IAssyChartService assyChartService, IEmailService emailService)
        {
            _email = emailService;
            _supervisorMobilityRepository = supervisorMobilityRepository ??
                throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
            _assyChartService = assyChartService;

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

            if (!await _supervisorMobilityRepository.OperationExistsAsync(jobObservation.OperationId))
            {
                return NotFound("No Operation");
            }

            var finalJobObservation = _mapper.Map<JobObservation>(jobObservation);
            if (finalJobObservation.OperationId == 0)
            {
                finalJobObservation.OperationId = null;
            }

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

            if (!await _supervisorMobilityRepository.OperationExistsAsync(jobObservationAndLup.OperationId))
            {
                return NotFound("No Operation");
            }

            var finalJobObservation = _mapper.Map<JobObservation>(jobObservationAndLup);
            if (finalJobObservation.OperationId == 0)
            {
                finalJobObservation.OperationId = null;
            }

            if (finalJobObservation.OperatorId == 0)
            {
                finalJobObservation.OperatorId = null;
            }
            _supervisorMobilityRepository.AddJobObservation(finalJobObservation);
            await _supervisorMobilityRepository.SaveChangesAsync();

            return Ok(finalJobObservation);
        }




        [HttpGet("filters")]
        public async Task<ActionResult<IEnumerable<JobObservationDto>>> GetJobObservationsByFilters(
            DateTime startDate,
            DateTime endDate,
            int plantId,
            int areaId,
            int distributionId,
            int operationId,
            int supervisorId,
            int status)
        {

            var allJobObservations = await _supervisorMobilityRepository.GetJobObservationsByFiltersAsync(startDate, endDate, plantId, areaId, distributionId, operationId, supervisorId, status);
            return Ok(_mapper.Map<IEnumerable<JobObservationDto>>(allJobObservations));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobObservationDto>>> GetAllJobObservationsAsync(bool includeTree = false, bool includePeople = false,
            bool includeLup = false, bool includeHistory = false, bool includeCkAnswers = false, int idPlant = 0, int idArea = 0, bool ForSosProgram = false,
            int year = 0, int SOSAnualId = 0, int idUser = 0)
        {

            var allJobObservations = await _supervisorMobilityRepository.GetAllJobObservationsAsync(includeTree, includePeople, includeLup, includeHistory, includeCkAnswers, idPlant, idArea, ForSosProgram, year, SOSAnualId, idUser);

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

            if (!await _supervisorMobilityRepository.OperationExistsAsync(jobObservationForUpdate.OperationId))
            {
                return NotFound("No Operation");
            }


            var jobObservationEntity = await _supervisorMobilityRepository.GetJobObservationAsync(jobObservationId, false);
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
                newnotify.NotificationText = $"The JobObservation with id: {jobObservationEntity.OperationId} was terminated by the user {auser}";
                newnotify.NotificationType = "FinishJobObservation";

                var notadd = await _assyChartService.CreateNotificationAsync(newnotify);
                //if (notadd != null)
                //{
                //    var emailMessage = _email.CreateEmailMessage(auser, "Este es un mensaje de prueba enviado desde job observation");
                //    _email.Send(emailMessage);
                //}
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
            if (jobObservationEntity.OperationId != jobObservationForUpdate.OperationId)
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

            if (jobObservationForUpdate.OperatorId == 0)
            {
                jobObservationForUpdate.OperatorId = null;
            }

            //Actualiza la jobobsevation
            _mapper.Map(jobObservationForUpdate, jobObservationEntity);

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
