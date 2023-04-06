using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.AssyChart;
using SupervisorMobility.API.Models.JobObservationDtos;
using SupervisorMobility.API.Models.OperationDtos;
using SupervisorMobility.API.Models.PlantDtos;
using SupervisorMobility.API.Services;
using System.Diagnostics;

namespace SupervisorMobility.API.Controllers
{
    [Route("api/jobobservations")]
    [ApiController]
    public class JobObservationController : ControllerBase
    {
        private readonly ISupervisorMobilityRepository _supervisorMobilityRepository;
        readonly IAssyChartService _assyChartService;
        private readonly IMapper _mapper;

        public JobObservationController(ISupervisorMobilityRepository supervisorMobilityRepository, IMapper mapper,
            IAssyChartService assyChartService)
        {
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

            _supervisorMobilityRepository.AddJobObservation(finalJobObservation);
            await _supervisorMobilityRepository.SaveChangesAsync();
            return Ok(finalJobObservation);
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobObservationDto>>> GetAllJobObservationsAsync(bool includeLup = false)
        {

            var allJobObservations = await _supervisorMobilityRepository.GetAllJobObservationsAsync(includeLup);
            if (includeLup)
            {
                return Ok(_mapper.Map<IEnumerable<JobObservationWithJustLupDto>>(allJobObservations));
            }
            return Ok(_mapper.Map<IEnumerable<JobObservationDto>>(allJobObservations));
        }

        [HttpGet("{jobObservationId}/history")]
        public async Task<ActionResult<IEnumerable<JobObservationHistoryDto>>> GetHistoryJobObservationsAsync(int jobObservationId)
        {
            var allHistory = await _supervisorMobilityRepository.GetAllHistoryJobObservationAsync(jobObservationId);
           
            return Ok(_mapper.Map<IEnumerable<JobObservationHistoryDto>>(allHistory));
        }

        [HttpGet("{jobObservationId}", Name = "GetJobObservation")]
        public async Task<IActionResult> GetJobObservation(int jobObservationId, bool includeLup = false)
        {
            //Find Job Observation type
            var jobObservation = await _supervisorMobilityRepository.GetJobObservationAsync(jobObservationId, includeLup);
            if (jobObservation == null)
            {
                return NotFound();
            }
            if (includeLup)
            {
                return Ok(_mapper.Map<JobObservationWithJustLupDto>(jobObservation));
            }

            return Ok(_mapper.Map<JobObservationDto>(jobObservation));
        }



        [HttpPut("{jobObservationId}")]
        public async Task<ActionResult> UpdateJobObservation(int jobObservationId, JobObservationForUpdateDto jobObservationForUpdate)
        {
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

            if(jobObservationEntity.Status == 4)
            {
                //crear notificacion
            }


            //Crear copia de la version actual en base de datos
            JobObservationVersion HistoryToAdd = await _assyChartService.CreateHistoryJobObservationAsync(jobObservationEntity);

            //Creo mensaje de cambios
            //Creamos mensaje de cambios
            string resumeChanges = "The following properties were changed: ";

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
            if (jobObservationEntity.DateStart != jobObservationForUpdate.dateStart)
            {
                resumeChanges += "dat eStart, ";
            }
            if (jobObservationEntity.DateEnd != jobObservationForUpdate.dateEnd)
            {
                resumeChanges += "date End, ";
            }
            if (jobObservationEntity.DateFinalized != jobObservationForUpdate.DateFinalized)
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
            if (jobObservationEntity.Time1HOE != jobObservationForUpdate.Time1HOE)
            {
                resumeChanges += "Timers 1, ";
            }
            if (jobObservationEntity.Time2HOE != jobObservationForUpdate.Time2HOE)
            {
                resumeChanges += "Timers 2, ";
            }
            if (jobObservationEntity.Models != jobObservationForUpdate.Models)
            {
                resumeChanges += "Models, ";
            }
            if (jobObservationEntity.Cicles != jobObservationForUpdate.Cicles)
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

            // Remove the trailing comma and space
            if (resumeChanges.EndsWith(", "))
            {
                resumeChanges = resumeChanges.Substring(0, resumeChanges.Length - 2);
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
