using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.Models.ILURegisterDtos;
using SupervisorMobility.API.Models.PATDtos;
using SupervisorMobility.API.Models.PlantDtos;
using SupervisorMobility.API.Models.SOS.SOSDistributionDtos;
using SupervisorMobility.API.Services;
using System.Diagnostics;

namespace SupervisorMobility.API.Controllers
{
    [Route("api/PAT")]
    [ApiController]

    public class PATController : ControllerBase
    {
        private readonly IMapper _mapper;
        readonly IAssyChartService _assyChartService;
        private readonly ISupervisorMobilityRepository _supervisorMobilityRepository;
        private readonly ISOS_ProcessRepository _ProcessRepository;

        public PATController(ISupervisorMobilityRepository supervisorMobilityRepository, IAssyChartService assyChartService,
            IMapper mapper, ISOS_ProcessRepository repository)
        {
            _ProcessRepository = repository;
            _assyChartService = assyChartService;
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

            if (result > 0)
            {
                return Ok(finalPat);
            }

            return NotFound();
        }

        [HttpPost("sosHub")]
        public async Task<ActionResult<PATDto>> GeneratePatSosHub(PATFotCreationDto patToCreate, int SOSHubCollection_Id)
        {

            if (patToCreate.PATid == 0)
            {
                
                patToCreate.CreationDate = DateTime.Now;
                patToCreate.IsActive = true;

                patToCreate.SOSHubId = SOSHubCollection_Id;

                PAT PatToCreate = _mapper.Map<PAT>(patToCreate);

                var createdResult = await _supervisorMobilityRepository.AddPat(PatToCreate);
                if (createdResult != null)
                    return Ok(PatToCreate);
                else
                    return BadRequest();
            }
            else
            {
                var patEntity = await _assyChartService.FetchPatAsync(patToCreate.PATid);
                if (patEntity == null)
                {
                    return NotFound();
                }

                PATForUpdateDto pat = _mapper.Map<PATForUpdateDto>(patToCreate);

                await _supervisorMobilityRepository.UpdatePAT(pat, patEntity);



                return Ok(patEntity);
            }

        }


        [HttpGet("{PATid}")]
        public async Task<ActionResult<PATDto>> getPatById(int PATid, bool includeCollections = false)
        {
            if (includeCollections)
            {
                var PatsWhitCollections = await _supervisorMobilityRepository.GetPat(PATid);
                return Ok(_mapper.Map<PATDto>(PatsWhitCollections));

            }
            else
            {
                var Pat = await _supervisorMobilityRepository
                                .GetPat(PATid);
                return Ok(_mapper.Map<PATwithoutNavigations>(Pat));

            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PATDto>>> getAllPats(bool includeCollections = false)
        {
            if (includeCollections)
            {
                var PatWithCollections = await _supervisorMobilityRepository.GetAllPATs();
                return Ok(_mapper.Map<IEnumerable<PATDto>>(PatWithCollections));
            }
            else
            {
                var Pats = await _supervisorMobilityRepository.GetAllPATs();
                return Ok(_mapper.Map<IEnumerable<PATwithoutNavigations>>(Pats));
            }
        }

        [HttpGet("SV/{idSV}")]
        public async Task<ActionResult<IEnumerable<PATDto>>> getAllPatsSV(
                    int idSV, bool includeCollections = false)
        {
            if (includeCollections)
            {
                var PatsWhitCollections = await _supervisorMobilityRepository.GetAllPATsOfSv(idSV);
                return Ok(_mapper.Map<IEnumerable<PATDto>>(PatsWhitCollections));

            }
            else
            {
                var Pats = await _supervisorMobilityRepository
                                .GetAllPATsOfSv(idSV);
                return Ok(_mapper.Map<IEnumerable<PATwithoutNavigations>>(Pats));

            }
        }

        [HttpGet("SSV/{idSSV}")]
        public async Task<ActionResult<IEnumerable<PATDto>>> getAllPatsSSV(
                    int idSSV, bool includeCollections = false)
        {
            if (includeCollections)
            {
                var PatsWhitCollections = await _supervisorMobilityRepository.GetAllPATsofSSV(idSSV);
                return Ok(_mapper.Map<IEnumerable<PATDto>>(PatsWhitCollections));

            }
            else
            {
                var Pats = await _supervisorMobilityRepository
                                .GetAllPATsofSSV(idSSV);
                return Ok(_mapper.Map<IEnumerable<PATwithoutNavigations>>(Pats));

            }
        }

        [HttpPut("{patId}")]
        public async Task<ActionResult> UpdatePat(int patId,
            PATForUpdateDto pat)
        {
            var patEntity = await _assyChartService.FetchPatAsync(patId);
            if (patEntity == null)
            {
                return NotFound();
            }

            await _supervisorMobilityRepository.UpdatePAT(pat, patEntity);

            return Ok();

        }

        //LeaderRecords

        [HttpPost("{patId}/LeadershipRecords")]
        public async Task<ActionResult> CreateLeadershipRecord(int patId,
          LeadershipRecordsForCreationDto leadershipRecordsForCreation)
        {
            var patEntity = await _assyChartService.FetchPatAsync(patId);
            if (patEntity == null)
            {
                return NotFound();
            }

            LeadershipRecord recordEntity = new LeadershipRecord();

            _mapper.Map(leadershipRecordsForCreation, recordEntity);

           var result = await _supervisorMobilityRepository.AddLeadershipRecordToPAT(patEntity, recordEntity);

            if(result > 0)
            {
                return Ok(recordEntity);
            }
            else
            {
                return NotFound($"Error: on create or save changes ");
            }

        }

        [HttpPut("{patId}/LeadershipRecords/")]
        public async Task<ActionResult> CreateLeadershipRecord(int patId,
         LeadershipRecordsForUpdateDto leadershipRecordForUpdate)
        {
            var patEntity = await _assyChartService.FetchPatAsync(patId);
            if (patEntity == null)
            {
                return NotFound($"Error: Pat by id {patId} Not Exist");
            }

           var result =  await _supervisorMobilityRepository.UpdateLeadershipRecordToPAT(patEntity, leadershipRecordForUpdate);

            if(result > 0 ) { 
            return Ok();
            }
            else
            {
                return NotFound($"Error: on update or save changes ");
            }

        }


    }
}
