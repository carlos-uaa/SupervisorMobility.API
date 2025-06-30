using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.HCIDtos;
using SupervisorMobility.API.Services;
using SupervisorMobility.API.Models.Users;
using SupervisorMobility.API.Models.HCICategoryDtos;
using SupervisorMobility.API.DataAccess.Entities.LUP;

namespace SupervisorMobility.API.Controllers.IS_Controllers
{
    [Route("api/HCI")]
    [ApiController]
    public class HCIController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ISupervisorMobilityRepository _supervisorMobilityRepository;
        private readonly IWebHostEnvironment _env;
        public HCIController(ISupervisorMobilityRepository supervisorMobilityRepository, IWebHostEnvironment env,
            IMapper mapper)
        {
            _supervisorMobilityRepository = supervisorMobilityRepository ??
                throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        [HttpPost]
        public async Task<ActionResult<HCIDto>> CreateNewHCI(CreateHCIDto hciForCreate)
        {
            HCI hciEntity = new();
            _mapper.Map(hciForCreate, hciEntity);

            hciEntity.User = await _supervisorMobilityRepository.GetUserAsync((int)hciForCreate.UserId);

            var entityhci = await _supervisorMobilityRepository.AddHCI(hciEntity);

            if (entityhci != null)
                return Ok(_mapper.Map<HCIDto>(hciEntity));
            else
                return BadRequest();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HCIDto>>> GetAllDataHCI(int LoginUserId, bool includeNavigation = false, bool includePeople = false, bool includeCommentaries = false, bool includeTransactions = false)
        {

            var entityhci = await _supervisorMobilityRepository.GetAllHCIs(LoginUserId, includeNavigation, includePeople, includeCommentaries, includeTransactions);
            if (entityhci != null)
                return Ok(_mapper.Map<List<HCIDto>>(entityhci));
            else
                return BadRequest();
        }

        [HttpGet("{hciId}")]
        public async Task<ActionResult<HCIDto>> GetHCI(int hciId, bool includeNavigation = false, bool includePeople = false, bool includeCommentaries = false, bool includeTransactions = false)
        {

            var entityhci = await _supervisorMobilityRepository.GetHCI(hciId, includeNavigation, includePeople, includeCommentaries, includeTransactions);
            if (entityhci != null)
                return Ok(entityhci);
            else
                return BadRequest();
        }

        [HttpPut("{hciId}")]
        public async Task<ActionResult<HCIDto>> UpdateHCI(int hciId, UpdateHCIDto hciForUpdate)
        {

            var entityhci = await _supervisorMobilityRepository.GetHCI(hciId, includePeople: true);

            var result = await _supervisorMobilityRepository.UpdateHCI(hciForUpdate, entityhci);

            if (result > 0)
                return Ok(entityhci);
            else
                return BadRequest();
        }

        [HttpDelete("{hciId}")]
        public async Task<ActionResult> DeleteHCI(int hciId)
        {
            var entityhci = await _supervisorMobilityRepository.GetHCI(hciId);

            var result = await _supervisorMobilityRepository.RemoveHCI(entityhci);

            if (result > 0)
                return Ok();
            else
                return BadRequest();
        }

        [HttpGet("NoHciUsers")]
        public async Task<ActionResult<UsersWithoutNavigationWithoutPeopleDetails>> GetUsersWithoutHCI()
        {

            var entityhci = await _supervisorMobilityRepository.GetUsersWithoutHci();
            if (entityhci != null)
                return Ok(_mapper.Map<List<UsersWithoutNavigationWithoutPeopleDetails>>(entityhci));
            else
                return BadRequest();
        }

        [HttpGet("Categories")]
        public async Task<ActionResult<IEnumerable<HCICategoryDto>>> GetHCICategories()
        {
            var resultlist = await _supervisorMobilityRepository.GetHCICategories();
            if (resultlist != null)
                return Ok(resultlist);
            else
                return BadRequest();
        }
    }
}
