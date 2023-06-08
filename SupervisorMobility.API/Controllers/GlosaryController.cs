using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Models.GlosaryDtos;
using SupervisorMobility.API.Services;

namespace SupervisorMobility.API.Controllers
{
    [Route("api/glosary")]
    [ApiController]
    public class GlosaryController : ControllerBase
    {
        readonly ISupervisorMobilityRepository _supervisorMobilityRepository;
        readonly IMapper _mapper;
        public GlosaryController(ISupervisorMobilityRepository supervisorMobilityRepository,
            IMapper mapper)
        {
            _supervisorMobilityRepository = supervisorMobilityRepository ??
                throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GlosaryDto>>> GetGlosary()
        {
            var glosaryEntities = await _supervisorMobilityRepository.GetGlosaryAsync();
            return Ok(_mapper.Map<IEnumerable<GlosaryDto>>(glosaryEntities));
        }

        [HttpGet("{glosaryWordId}", Name = "GetGlosaryWord")]
        public async Task<ActionResult> GetGlosaryWord(int glosaryWordId)
        {
            //Find Job Observation type
            var glosaryWord = await _supervisorMobilityRepository
                .GetGlosaryWordAsync(glosaryWordId);
            if (glosaryWord == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<GlosaryDto>(glosaryWord));
        }

        [HttpPost]
        public async Task<ActionResult<GlosaryDto>> CreateGlosaryWord(
            GlosaryForCreationDto glosaryWord)
        {
            //Mpa the pbject
            var finalGlosaryWord = _mapper.Map<Entities.Glosary>(glosaryWord);
            _supervisorMobilityRepository.AddGlosaryWord(finalGlosaryWord);
            await _supervisorMobilityRepository.SaveChangesAsync();

            return Ok(finalGlosaryWord);
            //var createGlosaryWordToReturn =
            //    _mapper.Map<GlosaryDto>(finalGlosaryWord);

            //return CreatedAtRoute("GetGlosary",
            //    new
            //    {
            //        glosaryId = createGlosaryWordToReturn.GlosaryWordId
            //    },
            //    createGlosaryWordToReturn);
        }


        [HttpPut("{glosaryWordId}")]
        public async Task<ActionResult> UpdateGlosaryWord(int glosaryWordId,
            GlosaryForUpdateDto glosaryWord)
        {
            var glosaryWordEntity = await _supervisorMobilityRepository.GetGlosaryWordAsync(glosaryWordId);
            if (glosaryWordEntity == null)
            {
                return NotFound();
            }

            _mapper.Map(glosaryWord, glosaryWordEntity);
            await _supervisorMobilityRepository.SaveChangesAsync();

            return Ok();

        }

        [HttpPatch("{glosaryWordId}")]
        public async Task<ActionResult> PartiallyUpdateGlosaryWord(
            int glosaryWordId,
            JsonPatchDocument<GlosaryForUpdateDto> patchDocumentGroup)
        {
            var glosaryWordEntity = await _supervisorMobilityRepository.GetGlosaryWordAsync(glosaryWordId);
            if (glosaryWordEntity == null)
            {
                return NotFound();
            }

            var glosaryWordToPatch = _mapper.Map<GlosaryForUpdateDto>(glosaryWordEntity);

            patchDocumentGroup.ApplyTo(glosaryWordToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(patchDocumentGroup))
            {
                return BadRequest();
            }

            _mapper.Map(glosaryWordToPatch, glosaryWordEntity);

            await _supervisorMobilityRepository.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{glosaryWordId}")]
        public async Task<ActionResult> DeleteGlosaryWord(int glosaryWordId)
        {
            var glosaryWordEntity = await _supervisorMobilityRepository.GetGlosaryWordAsync(glosaryWordId);
            if (glosaryWordEntity == null)
            {
                return NotFound();
            }

            _supervisorMobilityRepository.DeleteGlosaryWord(glosaryWordEntity);
            await _supervisorMobilityRepository.SaveChangesAsync();

            return Ok();
        }
    }
}
