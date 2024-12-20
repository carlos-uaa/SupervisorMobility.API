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
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

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
        public async Task<ActionResult<PATDto>> GeneratePatSosHub(PATFotCreationDto patToGenerate, int SOSHubCollection_Id)
        {

            if (patToGenerate.PATid == 0)
            {

                patToGenerate.CreationDate = DateTime.Now;
                patToGenerate.IsActive = true;

                patToGenerate.SOSHubId = SOSHubCollection_Id;

                PAT PatToCreate = _mapper.Map<PAT>(patToGenerate);

                PatToCreate.Supervisors.Clear();

                foreach(var usr in patToGenerate.Supervisors)
                {
                    PatToCreate.Supervisors.Add(await _supervisorMobilityRepository.GetUserAsync(usr.UserId));
                }

                var createdResult = await _supervisorMobilityRepository.AddPat(PatToCreate);


                if (createdResult != null)
                {
                    List<User> all_Users = new();

                    all_Users.AddRange(PatToCreate.Supervisors);
                    foreach(var usr in patToGenerate.Supervisors)
                    {
                        all_Users.AddRange(await _supervisorMobilityRepository.GetAllSubordinatesAsync(usr.UserId));
                        all_Users.Insert(0, await _supervisorMobilityRepository.GetUserAsync(usr.UserId));
                    }

                    PatToCreate.PatSubordinates = new List<PatSubordinate>();

                    foreach (User subordinate in all_Users)
                    {
                        PatSubordinate newSubordinate = new PatSubordinate();

                        newSubordinate.PatId = PatToCreate.PATid;
                        newSubordinate.UserId = subordinate.UserId;
                        newSubordinate.StartDate = new DateTime((int)PatToCreate.AplicationYear, 1, 1);

                        PatToCreate.PatSubordinates.Add(newSubordinate);
                    }

                    bool update = await _supervisorMobilityRepository.SaveChangesAsync();

                    if (update)
                    {
                        return Ok(PatToCreate);
                    }
                    else
                    {
                        Console.WriteLine("Error add subordinates");
                        return Ok(PatToCreate);
                    }

                }
                else
                    return BadRequest();
            }
            else
            {
                var patEntity = await _assyChartService.FetchPatAsync(patToGenerate.PATid);
                if (patEntity == null)
                {
                    return NotFound();
                }

                PATForUpdateDto pat = _mapper.Map<PATForUpdateDto>(patToGenerate);

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
                var Pat = await _supervisorMobilityRepository.GetPat(PATid);
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
        public async Task<ActionResult> UpdatePat(int patId,PATForUpdateDto pat)
        {
            var patEntity = await _assyChartService.FetchPatAsync(patId);
            if (patEntity == null)
            {
                return NotFound();
            }

            int resUpdate = await _supervisorMobilityRepository.UpdatePAT(pat, patEntity);

            if (resUpdate > 0)
            {
                return Ok();    
            }
            else
            {
                return NotFound();
            }

        }

        //LeaderRecords

        [HttpDelete("{patId}")]
        public async Task<ActionResult<int>> RemovePat(int patId)
        {

            var PatEntity = await _supervisorMobilityRepository.GetPat(patId);
            var result = await _supervisorMobilityRepository.DeletePat(PatEntity);

            if (result > 0)
                return Ok(PatEntity);
            else
                return BadRequest("something wrong");
        }

        


    }
}
