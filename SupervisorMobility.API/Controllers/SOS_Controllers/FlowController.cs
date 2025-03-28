using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.Models.SOS.SOSCombinationDtos;
using SupervisorMobility.API.Models.SOS.SOSCombinationLogbookDtos;
using SupervisorMobility.API.Models.SOS.SOSFlowDtos;
using SupervisorMobility.API.Models.SOS.SOSFlowLogbookDtos;
using SupervisorMobility.API.Models.SOS.TurnDtos;
using System.Diagnostics;

namespace SupervisorMobility.API.Controllers.SOS_Controllers
{
    [Route("api/SOS/Flow")]
    [ApiController]
    public class FlowController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ISOS_ProcessRepository _ProcessRepository;
        private readonly IWebHostEnvironment _env;
        public FlowController(IWebHostEnvironment env, IMapper mapper, ISOS_ProcessRepository repository)
        {
            _ProcessRepository = repository;
            _mapper = mapper ??
                  throw new ArgumentNullException(nameof(mapper));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        [HttpPost]
        public async Task<ActionResult<SOSFlowDto>> GenerateFlow(SOSFlowForCreateDto sOSFlowToCreate, int SOSHubCollection_Id)
        {
            SOSHub SOSEntity = await _ProcessRepository.GetSOSHub(SOSHubCollection_Id, includeInformation: true);

            if (sOSFlowToCreate.SOSFlowId == 0)
            {
                sOSFlowToCreate.IsActive = true;

                sOSFlowToCreate.SOSHubId = SOSHubCollection_Id;


                SOSFlow FlowToCreate = _mapper.Map<SOSFlow>(sOSFlowToCreate);

                if (FlowToCreate.ReviewerHSId <= 0)
                {
                    FlowToCreate.ReviewerHSId = null;
                }


                var createdResult = await _ProcessRepository.CreateSOSFlow(FlowToCreate);
                if (createdResult != null)
                    return Ok(FlowToCreate);
                else
                    return BadRequest();
            }
            else
            {
                //only add revision
                SOSFlow _sosFlow = await _ProcessRepository.GetSOSFlow(sOSFlowToCreate.SOSFlowId, true, true, true, true);

                // si el hys anterior es diferente update
                // si el anterior es null y hay un id actualizar
                if (sOSFlowToCreate.ReviewerHSId <= 0)
                {
                    sOSFlowToCreate.ReviewerHSId = null;
                }
                else if (sOSFlowToCreate.ReviewerHSId != _sosFlow.ReviewerHSId)
                {
                    //update
                }

                SOSFlowLogbook _logbookToCreate = _mapper.Map<SOSFlowLogbook>(sOSFlowToCreate.FlowLogbooks?.Last());
                _logbookToCreate.SOSFlowId = _sosFlow.SOSFlowId;

                var resultAddSections = await _ProcessRepository.CreateSOSFlowLogbook(_logbookToCreate);

                if (resultAddSections > 0)
                {
                    Debug.WriteLine("SOSFlowLogbook añadidas con exito");
                    await _ProcessRepository.AddSOSFlowLogbookToSOSFlow(_sosFlow, _logbookToCreate);
                }
                else
                {
                    Debug.WriteLine("Error Sections añadidos");
                    return BadRequest();
                }



                return Ok(_sosFlow);
            }

        }//New revision

        [HttpGet("{id}", Name = "GetSOSFlow")]
        public async Task<ActionResult<SOSFlowDto>> GetSOSFlow(int id, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false, bool includeImagesSOS = false, bool includePeople = false)
        {

            var SOSFlow = await _ProcessRepository.GetSOSFlow(id, includeImages, includeNotes, includeLogbooks, includeSOS, includeImagesSOS, includePeople);
            if (SOSFlow == null)
            {
                return NotFound("SOSFlow not found!");
            }

            return Ok(_mapper.Map<SOSFlowDto>(SOSFlow));
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<SOSFlowDto>>> GetAllSOSFlow(bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false)
        {

            var CheckpointEntities = await _ProcessRepository.GetAllSOSFlow(includeImages, includeNotes, includeLogbooks, includeSOS);
            if (CheckpointEntities == null)
            {
                return NotFound("Get All Sos Analisis not found!");
            }

            return Ok(_mapper.Map<IEnumerable<SOSFlowDto>>(CheckpointEntities));
        }

        //Update
        [HttpPut("{sosFlow_Id}")]
        public async Task<ActionResult> UpdateSOSFlow(int sosFlow_Id, SOSFlowForUpdateDto sosUpdateEntity)
        {
            List<Turn> Bkup_Turn = new List<Turn>();
            List<SOSFlowLogbook> Bkup_FlowLogbook = new List<SOSFlowLogbook>();

            // Filtrar nuevos FlowLogbooks
            List<SOSFlowLogbookForUpdateDto> filteredFlowLogbooksList = sosUpdateEntity.FlowLogbooks.Where(t => t.SOSFlowLogbookId <= 0).ToList();
            // Filtrar nuevos Turnos


            // Remover nuevos FlowLogbooks de la lista principal para evitar duplicados
            if (filteredFlowLogbooksList.Any())
            {
                sosUpdateEntity.FlowLogbooks.RemoveAll(t => t.SOSFlowLogbookId == null || t.SOSFlowLogbookId <= 0);

                // Mapear nuevas norms/standars
                List<SOSFlowLogbook> newSOSFlowLogbook = _mapper.Map<List<SOSFlowLogbook>>(filteredFlowLogbooksList);

                foreach (var FlowLogbook in newSOSFlowLogbook)
                {
                    FlowLogbook.SOSFlowLogbookId = 0;
                    FlowLogbook.IsActive = true;
                }

                var resultAddSOSFlowLogbook = await _ProcessRepository.AddRangeSOSFlowLogbook(newSOSFlowLogbook);

                if (resultAddSOSFlowLogbook != null)
                {
                    Debug.WriteLine("FlowLogbooks añadidos con exitop");
                    Bkup_FlowLogbook.AddRange(resultAddSOSFlowLogbook);
                }
                else
                {
                    Debug.WriteLine("Error FlowLogbooks añadidos");
                }
            }


           


            SOSFlow _sosFlow = await _ProcessRepository.GetSOSFlow(sosFlow_Id, true, true, true);

            ////Aqui va el historico de ser necesario en  un futuro 

            ////Ejemplo de uso 
            ////Compare genera un string que menciona las diferencias
            ////string jsonResult = CompareAndGenerateJson(_mapper.Map<SOSHubForUpdateDto>(entitySOSHub), _SOSHubForUpdate);
            ////se crea un entity 
            ////SOSHubHistory newHistory = new SOSHubHistory();
            ////_mapper.Map(entitySOSHub, newHistory);
            ////newHistory.VersionChanges = jsonResult;
            ////se almacena la entity anterior y se le añade el resumen de cambios
            ////await _ProcessRepository.CreateHistorySOScollection(newHistory);



            foreach (var logbook in sosUpdateEntity.FlowLogbooks)
            {
                var flowUpdate = await _ProcessRepository.UpdateFlowLogbook(logbook);
                SOSFlowLogbook flowBkaux = await _ProcessRepository.GetSOSFlowLogbookById(logbook.SOSFlowLogbookId);
                Bkup_FlowLogbook.Add(flowBkaux);
            }


            //Nulleamos el update para evitar errores
            sosUpdateEntity.FlowLogbooks = null;

            await _ProcessRepository.SOSDataRemoveAllSOSFlowLogbookFromSOSFlow(_sosFlow);

            var result = await _ProcessRepository.UpdateSOSFlow(sosUpdateEntity, _sosFlow);

            // Volver a añádir bkup

            //Flow Logbook
            if (Bkup_FlowLogbook.Any())
            {
                foreach (SOSFlowLogbook logbook in Bkup_FlowLogbook)
                {
                    await _ProcessRepository.AddSOSFlowLogbookToSOSFlow(_sosFlow, logbook);
                }
            }

         
            if (result != null)
            {
                return Ok(_sosFlow);
            }
            else
                return BadRequest();

        }//end Update 



       


        [HttpDelete("{SOSFlowId}")]
        public async Task<ActionResult<int>> RemoveSOSHub(int SOSFlowId)
        {
            var result = await _ProcessRepository.RemoveSOSFlow(SOSFlowId);

            if (result > 0)
                return Ok();
            else
                return BadRequest("something wrong");
        }

    }
}
