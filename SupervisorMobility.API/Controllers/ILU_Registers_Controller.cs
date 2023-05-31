using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.AreaDtos;
using SupervisorMobility.API.Models.PATDtos;
using SupervisorMobility.API.Services;

namespace SupervisorMobility.API.Controllers
{
        [Route("api/ILU_Registers")]
        [ApiController]

    public class ILU_Registers_Controller : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ISupervisorMobilityRepository _supervisorMobilityRepository;

        public ILU_Registers_Controller(ISupervisorMobilityRepository supervisorMobilityRepository,
            IMapper mapper)
        {
            _supervisorMobilityRepository = supervisorMobilityRepository ??
                throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }



        //[HttpPost]
        //public async Task<ActionResult> AddNewPat(PATFotCreationDto PatForCreate)
        //{
        //    var finalPat = _mapper.Map<PAT>(PatForCreate);
        //    var result = await _supervisorMobilityRepository.AddPat(finalPat);
            
        //    if(result > 0){
        //        return Ok(finalPat);
        //    }

        //    return NotFound();
        //}


        //[HttpGet("{PATid}")]
        //public async Task<ActionResult<PATDto>> getPatById(
        //          int PATid, bool includeCollections = false)
        //{
        //    if (includeCollections)
        //    {
        //        var PatsWhitCollections = await _supervisorMobilityRepository.GetPat(PATid);
        //        return Ok(_mapper.Map<PATDto>(PatsWhitCollections));

        //    }
        //    else
        //    {
        //        var Pat = await _supervisorMobilityRepository
        //                        .GetPat(PATid);
        //        return Ok(_mapper.Map<PATwithoutNavigations>(Pat));

        //    }
        //}

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<PATDto>>> getAllPats(bool includeCollections = false)
        //{
        //    if (includeCollections)
        //    {
        //        var PatWithCollections = await _supervisorMobilityRepository.GetAllPATs();
        //        return Ok(_mapper.Map<IEnumerable<PATDto>>(PatWithCollections));
        //    }
        //    else
        //    {
        //        var Pats = await _supervisorMobilityRepository.GetAllPATs();
        //        return Ok(_mapper.Map<IEnumerable<PATwithoutNavigations>>(Pats));
        //    }
        //}

        //[HttpGet("SV/{idSV}")]
        //public async Task<ActionResult<IEnumerable<PATDto>>> getAllPatsSV(
        //            int idSV, bool includeCollections = false)
        //{
        //    if (includeCollections)
        //    {
        //        var PatsWhitCollections = await _supervisorMobilityRepository.GetAllPATsOfSv(idSV);
        //        return Ok(_mapper.Map<IEnumerable<PATDto>>(PatsWhitCollections));

        //    }
        //    else
        //    {
        //        var Pats = await _supervisorMobilityRepository
        //                        .GetAllPATsOfSv(idSV);
        //        return Ok(_mapper.Map<IEnumerable<PATwithoutNavigations>>(Pats));

        //    }
        //}

        //[HttpGet("SSV/{idSSV}")]
        //public async Task<ActionResult<IEnumerable<PATDto>>> getAllPatsSSV(
        //            int idSSV, bool includeCollections = false)
        //{
        //    if (includeCollections)
        //    {
        //        var PatsWhitCollections = await _supervisorMobilityRepository.GetAllPATsofSSV(idSSV);
        //        return Ok(_mapper.Map<IEnumerable<PATDto>>(PatsWhitCollections));

        //    }
        //    else
        //    {
        //        var Pats = await _supervisorMobilityRepository
        //                        .GetAllPATsofSSV(idSSV);
        //        return Ok(_mapper.Map<IEnumerable<PATwithoutNavigations>>(Pats));

        //    }
        //}
    }
}
