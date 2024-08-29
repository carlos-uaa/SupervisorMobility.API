using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.Models.CommentaryDtos;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.SOS.SOSAnalysisDtos;
using SupervisorMobility.API.Models.SOS.SOSCombinationDtos;
using SupervisorMobility.API.Models.SOS.SOSCombinationLogbookDtos;
using SupervisorMobility.API.Models.SOS.SOSDistributionDtos;
using SupervisorMobility.API.Models.SOS.SOSDistributionLogbookDtos;
using SupervisorMobility.API.Models.SOS.SOSTimeDtos;
using SupervisorMobility.API.Models.SOS.TurnDtos;
using System.Diagnostics;

namespace SupervisorMobility.API.Controllers.SOS_Controllers
{
    [Route("api/SOS/Combination")]
    [ApiController]
    public class CombinationController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        private readonly ISOS_ProcessRepository _ProcessRepository;
        public CombinationController(IWebHostEnvironment env, IMapper mapper, ISOS_ProcessRepository repository)
        {
            _ProcessRepository = repository;
            _mapper = mapper ??
                  throw new ArgumentNullException(nameof(mapper));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        [HttpPost]
        public async Task<ActionResult<SOSCombinationDto>> GenerateCombination(SOSCombinationForCreateDto sOSCombinationToCreate, int SOSHubCollection_Id)
        {
            SOSHub SOSEntity = await _ProcessRepository.GetSOSHub(SOSHubCollection_Id, includeInformation: true);

            if (sOSCombinationToCreate.SOSCombinationId == 0)
            {
                sOSCombinationToCreate.IsActive = true;

                sOSCombinationToCreate.SOSHubId = SOSHubCollection_Id;


                SOSCombination CombinationToCreate = _mapper.Map<SOSCombination>(sOSCombinationToCreate);

                var createdResult = await _ProcessRepository.CreateSOSCombination(CombinationToCreate);
                if (createdResult != null)
                    return Ok(CombinationToCreate);
                else
                    return BadRequest();
            }
            else
            {
                //only add revision
                SOSCombination _sosCombination = await _ProcessRepository.GetSOSCombination(sOSCombinationToCreate.SOSCombinationId, true, true, true, true );

                SOSCombinationLogbook _logbookToCreate = _mapper.Map<SOSCombinationLogbook>(sOSCombinationToCreate.CombinationLogbooks?.Last());
                _logbookToCreate.SOSCombinationId = _sosCombination.SOSCombinationId;

                var resultAddSections = await _ProcessRepository.CreateSOSCombinationLogbook(_logbookToCreate);

                if (resultAddSections > 0)
                {
                    Debug.WriteLine("SOSCombinationLogbook añadidas con exito");
                    await _ProcessRepository.AddSOSCombinationLogbookToSOSCombination(_sosCombination, _logbookToCreate);
                }
                else
                {
                    Debug.WriteLine("Error Sections añadidos");
                    return BadRequest();
                }



                return Ok(_sosCombination);
            }

        }//New revision

        [HttpGet("{id}", Name = "GetSOSCombination")]
        public async Task<ActionResult<SOSCombinationDto>> GetSOSCombination(int id, bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false, bool includeImagesSOS = false)
        {

            var SOSCombination = await _ProcessRepository.GetSOSCombination(id, includeImages, includeNotes, includeLogbooks,  includeSOS, includeImagesSOS);
            if (SOSCombination == null)
            {
                return NotFound("SOSCombination not found!");
            }

            return Ok(_mapper.Map<SOSCombinationDto>(SOSCombination));
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<SOSCombinationDto>>> GetAllSOSCombination(bool includeImages = false, bool includeNotes = false, bool includeLogbooks = false, bool includeSpecialCases = false, bool includeSOS = false)
        {

            var CheckpointEntities = await _ProcessRepository.GetAllSOSCombination(includeImages, includeNotes, includeLogbooks, includeSOS);
            if (CheckpointEntities == null)
            {
                return NotFound("Get All Sos Analisis not found!");
            }

            return Ok(_mapper.Map<IEnumerable<SOSCombinationDto>>(CheckpointEntities));
        }

        //Update
        [HttpPut("{sosCombination_Id}")]
        public async Task<ActionResult> UpdateSOSCombination(int sosCombination_Id, SOSCombinationForUpdateDto sosUpdateEntity)
        {
            List<Turn> Bkup_Turn = new List<Turn>();
            List<SOSCombinationLogbook> Bkup_CombinationLogbook = new List<SOSCombinationLogbook>();

          // Filtrar nuevos CombinationLogbooks
            List<SOSCombinationLogbookForUpdateDto> filteredCombinationLogbooksList = sosUpdateEntity.CombinationLogbooks.Where(t => t.SOSCombinationLogbookId <= 0).ToList();
           // Filtrar nuevos Turnos
            List<TurnForUpdateDto> filteredTurnList = sosUpdateEntity.Turns.Where(t => t.TurnId <= 0).ToList();


            // Remover nuevos CombinationLogbooks de la lista principal para evitar duplicados
            if (filteredCombinationLogbooksList.Any())
            {
                sosUpdateEntity.CombinationLogbooks.RemoveAll(t => t.SOSCombinationLogbookId == null || t.SOSCombinationLogbookId <= 0);

                // Mapear nuevas norms/standars
                List<SOSCombinationLogbook> newSOSCombinationLogbook = _mapper.Map<List<SOSCombinationLogbook>>(filteredCombinationLogbooksList);

                foreach (var CombinationLogbook in newSOSCombinationLogbook)
                {
                    CombinationLogbook.SOSCombinationLogbookId = 0;
                    CombinationLogbook.IsActive = true;
                }

                var resultAddSOSCombinationLogbook = await _ProcessRepository.AddRangeSOSCombinationLogbook(newSOSCombinationLogbook);

                if (resultAddSOSCombinationLogbook != null)
                {
                    Debug.WriteLine("CombinationLogbooks añadidos con exitop");
                    Bkup_CombinationLogbook.AddRange(resultAddSOSCombinationLogbook);
                }
                else
                {
                    Debug.WriteLine("Error CombinationLogbooks añadidos");
                }
            }


            //Turnos
            if (filteredTurnList.Any())
            {
                sosUpdateEntity.Turns.RemoveAll(t => t.TurnId == null || t.TurnId <= 0);

                // Mapear nuevas tiempos
                List<Turn> newTurn = _mapper.Map<List<Turn>>(filteredTurnList);

                foreach (var time in newTurn)
                {
                    time.TurnId = 0;
                    //time.IsActive = true;
                }

                var resultAddTurn = await _ProcessRepository.AddRangeTurns(newTurn);

                if (resultAddTurn != null)
                {
                    Debug.WriteLine("Add Turn añadidos con exito");
                    Bkup_Turn.AddRange(resultAddTurn);
                }
                else
                {
                    Debug.WriteLine("Error Add Turn añadidos");
                }
            }


            SOSCombination _sosCombination = await _ProcessRepository.GetSOSCombination(sosCombination_Id, true, true, true, true);

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



            //Crear bkup de datos relacionados
      
            foreach (var logbook in sosUpdateEntity.CombinationLogbooks)
            {
                SOSCombinationLogbook CombinationBkaux = await _ProcessRepository.GetSOSCombinationLogbookById(logbook.SOSCombinationLogbookId);
                _mapper.Map(logbook, CombinationBkaux);
                Bkup_CombinationLogbook.Add(CombinationBkaux);
            }

            foreach (var turn in sosUpdateEntity.Turns)
            {
                var turnUpdate = await _ProcessRepository.UpdateTurn(turn);
                Turn turnBkaux = await _ProcessRepository.GetTurnById(turn.TurnId);
                Bkup_Turn.Add(turnBkaux);
            }

            //Nulleamos el update para evitar errores
            sosUpdateEntity.Turns = null;
            sosUpdateEntity.CombinationLogbooks = null;

            await _ProcessRepository.RemoveAllTurnsFromSOSCombination(_sosCombination);
            await _ProcessRepository.SOSDataRemoveAllSOSCombinationLogbookFromSOSCombination(_sosCombination);

            var result = await _ProcessRepository.UpdateSOSCombination(sosUpdateEntity, _sosCombination);

            // Volver a añádir bkup

            //Combination Logbook
            if (Bkup_CombinationLogbook.Any())
            {
                foreach (SOSCombinationLogbook logbook in Bkup_CombinationLogbook)
                {
                    await _ProcessRepository.AddSOSCombinationLogbookToSOSCombination(_sosCombination, logbook);
                }
            }

            //turns
            if (Bkup_Turn.Any())
            {
                foreach (Turn turn in Bkup_Turn)
                {
                    await _ProcessRepository.AddTurnToSOSCombination(_sosCombination, turn);
                }
            }

            if (result != null)
            {
                return Ok(_sosCombination);
            }
            else
                return BadRequest();

        }//end Update 



        [HttpDelete("{SOSAnaysisId}")]
        public async Task<ActionResult<int>> RemoveSOSHub(int SOSAnaysisId)
        {
            var result = await _ProcessRepository.RemoveSOSDistribution(SOSAnaysisId);

            var SOSHub = await _ProcessRepository.GetSOSHub(SOSAnaysisId);

            if (result > 0)
                return Ok(SOSHub);
            else
                return BadRequest("something wrong");
        }


        //ilustrations

        [HttpPost("Ilustrations/{Combination_id}")]
        public async Task<ActionResult<FileUpload>> UploadIlustrations(int Combination_id, IFormFile file)
        {

            var uploadResult = new FileUploadForCreationDto();
            string trustedFileNameForStorage = string.Empty;
            var unstrustedFileName = file.FileName;

            trustedFileNameForStorage = Path.GetRandomFileName();

            var path = Path.Combine(_env.ContentRootPath, "uploads\\SOSCombination\\Ilustrations", trustedFileNameForStorage);
            // Asegurarse de que el directorio de destino exista
            var directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }


            await using FileStream fs = new(path, FileMode.Create);
            await file.CopyToAsync(fs);

            uploadResult.FileName = unstrustedFileName;
            uploadResult.StorageFileName = trustedFileNameForStorage;
            uploadResult.ContentType = file.ContentType;
            uploadResult.UploadDate = DateTime.Now;
            uploadResult.IsActive = true;

            var fileToReturn = await _ProcessRepository.CreateFileAsync(uploadResult);

            await _ProcessRepository.AddIlustrationToSOSCombination(Combination_id, fileToReturn);
            await _ProcessRepository.SaveChangesAsync();

            return Ok(fileToReturn);
        }

        [HttpGet("Ilustrations/{fileid}")]
        public async Task<IActionResult> DownloadIlustrations(int fileid)
        {
            var FileInfo = await _ProcessRepository.FetchFileAsync(fileid);

            if (FileInfo is not null)
            {
                var path = Path.Combine(_env.ContentRootPath, "uploads\\SOSCombination\\Ilustrations", FileInfo.StorageFileName);

                var memory = new MemoryStream();
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;

                var result = File(memory, FileInfo.ContentType, Path.GetFileName(path));
                result.EnableRangeProcessing = true;

                return result;
            }
            return NotFound("Error File download");
        }

        [HttpDelete("Ilustrations/{SOS_SOSCombination_id}/remove/{ImageFile_id}")]
        public async Task<ActionResult<int>> RemoveImage(int SOS_SOSCombination_id, int ImageFile_id)
        {
            var result = await _ProcessRepository.RemoveIlustrationFromSOSCombination(SOS_SOSCombination_id, ImageFile_id);

            if (result > 0)
                return Ok();
            else
                return BadRequest("something went wrong");
        }
    }
}
