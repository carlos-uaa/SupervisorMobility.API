using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.AreaDtos;
using SupervisorMobility.API.Models.PATDtos;
using SupervisorMobility.API.Services;

namespace SupervisorMobility.API.Controllers
{
        [Route("api/PAT")]
        [ApiController]

    public class PATController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ISupervisorMobilityRepository _supervisorMobilityRepository;

        public PATController(ISupervisorMobilityRepository supervisorMobilityRepository,
            IMapper mapper)
        {
            _supervisorMobilityRepository = supervisorMobilityRepository ??
                throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpPost]
        public async Task<ActionResult> AddNewPat(PATFotCreationDto PatForCreate)
        {
            var finalPat = _mapper.Map<PAT>(PatForCreate);
            var result = await _supervisorMobilityRepository.AddPat(finalPat);
            
            if(result > 0){
                return Ok(finalPat);
            }

            return NotFound();
        }


        [HttpGet("{PATid}")]
        public async Task<ActionResult<IEnumerable<PATDto>>> getPatById(
                  int PATid, bool includeCollections = false)
        {
            if (includeCollections)
            {
                var PatsWhitCollections = await _supervisorMobilityRepository.GetPat(PATid);
                return Ok(_mapper.Map<IEnumerable<PATDto>>(PatsWhitCollections));

            }
            else
            {
                var Pat = await _supervisorMobilityRepository
                                .GetPat(PATid);
                return Ok(_mapper.Map<IEnumerable<PATwithoutNavigations>>(Pat));

            }
        }
        [HttpGet("SV/{PATid}")]
        public async Task<ActionResult<IEnumerable<PATDto>>> getAllPatsSV(
                    int idSup, bool includeCollections = false)
        {
            if (includeCollections)
            {
                var PatsWhitCollections = await _supervisorMobilityRepository.GetAllPATsOfSv(idSup);
                return Ok(_mapper.Map<IEnumerable<PATDto>>(PatsWhitCollections));

            }
            else
            {
                var Pats = await _supervisorMobilityRepository
                                .GetAllPATsOfSv(idSup);
                return Ok(_mapper.Map<IEnumerable<PATwithoutNavigations>>(Pats));

            }
        }

        [HttpGet("SSV/{PATid}")]
        public async Task<ActionResult<IEnumerable<PATDto>>> getAllPatsSSV(
                    int idSup, bool includeCollections = false)
        {
            if (includeCollections)
            {
                var PatsWhitCollections = await _supervisorMobilityRepository.GetAllPATsofSSV(idSup);
                return Ok(_mapper.Map<IEnumerable<PATDto>>(PatsWhitCollections));

            }
            else
            {
                var Pats = await _supervisorMobilityRepository
                                .GetAllPATsofSSV(idSup);
                return Ok(_mapper.Map<IEnumerable<PATwithoutNavigations>>(Pats));

            }
        }
    }
}
