using AutoMapper;
using CsvHelper.Configuration.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.Models.ILURegisterDtos;
using SupervisorMobility.API.Models.NotificationDtos;
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
        private readonly INotificationService _notificationService;

        public PATController(ISupervisorMobilityRepository supervisorMobilityRepository, IAssyChartService assyChartService,
            IMapper mapper, ISOS_ProcessRepository repository, INotificationService notificationService)
        {
            _ProcessRepository = repository;
            _assyChartService = assyChartService;
            _supervisorMobilityRepository = supervisorMobilityRepository ??
                throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
            _notificationService = notificationService ??
                throw new ArgumentNullException(nameof(notificationService));
        }

        [HttpPost]
        public async Task<ActionResult> AddNewPat(PATFotCreationDto PatForCreate)
        {
            var finalPat = _mapper.Map<PAT>(PatForCreate);
            finalPat.IsActive = true;

            List<User> users = finalPat.Supervisors.ToList();
            finalPat.Supervisors = new List<User>();

            foreach (var usr in users)
            {
                finalPat.Supervisors.Add(await _supervisorMobilityRepository.GetUserAsync(usr.UserId));
            }

            var result = await _supervisorMobilityRepository.AddPat(finalPat);

            if (result > 0)
            {


                if (finalPat != null)
                {
                    List<User> all_Users = new();

                    all_Users.AddRange(finalPat.Supervisors);
                    foreach (var usr in finalPat.Supervisors)
                    {
                        all_Users.AddRange(await _supervisorMobilityRepository.GetAllSubordinatesAsync(usr.UserId));
                    }

                    finalPat.PatSubordinates = new List<PatSubordinate>();

                    foreach (User subordinate in all_Users)
                    {
                        PatSubordinate newSubordinate = new PatSubordinate();

                        newSubordinate.PatId = finalPat.PATid;
                        newSubordinate.UserId = subordinate.UserId;
                        
                        PatSubordinateDates newDate = new PatSubordinateDates();
                        newDate.StartDate = new DateTime((int)finalPat.AplicationYear, 1, 1);

                        newSubordinate.PatSubordinateDates.Add(newDate);
                        finalPat.PatSubordinates.Add(newSubordinate);
                    }

                    bool update = await _supervisorMobilityRepository.SaveChangesAsync();

                    return Ok(new
                    {
                        success = true,
                        partialFailure = !update,
                        message = update
                         ? "Proceso completado correctamente."
                         : "El proceso se completó, pero no se pudieron agregar los subordinados.",
                        data = finalPat
                    });

                }
                else
                    return BadRequest();

               
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


                if (createdResult > 0)
                {
                    List<User> all_Users = new();

                    all_Users.AddRange(PatToCreate.Supervisors);
                    foreach(var usr in patToGenerate.Supervisors)
                    {
                        all_Users.AddRange(await _supervisorMobilityRepository.GetAllSubordinatesAsync(usr.UserId));
                    }

                    PatToCreate.PatSubordinates = new List<PatSubordinate>();

                    foreach (User subordinate in all_Users)
                    {
                        PatSubordinate newSubordinate = new PatSubordinate();

                        newSubordinate.PatId = PatToCreate.PATid;
                        newSubordinate.UserId = subordinate.UserId;
                      
                        PatSubordinateDates newDate = new PatSubordinateDates();
                        newDate.StartDate = new DateTime((int)PatToCreate.AplicationYear, 1, 1);

                        newSubordinate.PatSubordinateDates.Add(newDate);

                        PatToCreate.PatSubordinates.Add(newSubordinate);
                    }

                    bool update = await _supervisorMobilityRepository.SaveChangesAsync();

                    if (update)
                    {
                        int notifyUserId = PatToCreate.Supervisors.FirstOrDefault()?.UserId ?? 1;
                        await _notificationService.CreateNotificationAsync(new NotificationToCreateDto
                        {
                            MadeBy = "SM Mobility",
                            NotificationType = "PAT Created",
                            NotificationText = $"PAT (ID: {PatToCreate.PATid}) has been generated for SOS Hub (ID: {SOSHubCollection_Id}).",
                            UserId = notifyUserId,
                            IsActive = true,
                            IsAccepted = true,
                            EntryDate = DateTime.Now
                        });

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

                int updateResult = await _supervisorMobilityRepository.UpdatePAT(pat, patEntity);

                if (updateResult > 0)
                {
                    int notifyUserId = patEntity.Supervisors?.FirstOrDefault()?.UserId
                        ?? patToGenerate.Supervisors?.FirstOrDefault()?.UserId
                        ?? 1;

                    await _notificationService.CreateNotificationAsync(new NotificationToCreateDto
                    {
                        MadeBy = "SM Mobility",
                        NotificationType = "PAT Updated",
                        NotificationText = $"PAT (ID: {patEntity.PATid}) has been updated.",
                        UserId = notifyUserId,
                        IsActive = true,
                        IsAccepted = true,
                        EntryDate = DateTime.Now
                    });
                }



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


        //GetPATByJob
        [HttpGet("JOB/{idJob}")]
        public async Task<ActionResult<int>> getPatByJobId(int idJob, [FromQuery] int plantid, [FromQuery] int areaid)
        {
            var registry = await _supervisorMobilityRepository.GetILUIdByJobId(idJob);
            if (registry == null)
            {
                return NotFound();
            }

            var PatId = await _supervisorMobilityRepository.GetPatByRegister(registry, plantid, areaid);
            if(PatId == null)
            {
                return NotFound();
            }

            return Ok(PatId);
        }

    }
}
