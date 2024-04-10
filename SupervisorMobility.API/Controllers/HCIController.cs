using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.Services;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.HCIDtos;

namespace SupervisorMobility.API.Controllers
{
    [ApiController]
    [Route("api/HCI")]
    public class HCIController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ISupervisorMobilityRepository _supervisorMobilityRepository;
        private readonly SupervisorMobilityContext _context;
        private readonly IWebHostEnvironment _env;
        public HCIController(ISupervisorMobilityRepository supervisorMobilityRepository, SupervisorMobilityContext context, IWebHostEnvironment env,
            IMapper mapper)
        {
            _supervisorMobilityRepository = supervisorMobilityRepository ??
                throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        [HttpPost]
        public async Task<ActionResult<HCIDto>> CreateHCI(CreateHCIDto hciForCreate)
        {
            HCI hciEntity = _mapper.Map<HCI>(hciForCreate);

            var entityhci = await _supervisorMobilityRepository.AddHCI(hciEntity);
            if (entityhci != null)
                return Ok(hciEntity);
            else
                return BadRequest(); ;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HCIDto>>> GetAllDataHCI(bool includeNavigation = false, bool includePeople = false, bool includeEvidences= false, bool includeTransactions = false)
        {

            var entityhci = await _supervisorMobilityRepository.GetAllHCIs(includeNavigation, includePeople, includeEvidences, includeTransactions);
            if (entityhci != null)
                return Ok(entityhci);
            else
                return BadRequest(); ;
        }

        [HttpGet("{hciId}")]
        public async Task<ActionResult<HCIDto>> GetHCI(int hciId, bool includeNavigation = false, bool includePeople = false, bool includeEvidences = false, bool includeTransactions = false)
        {

            var entityhci = await _supervisorMobilityRepository.GetHCI(hciId, includeNavigation, includePeople, includeEvidences, includeTransactions);
            if (entityhci != null)
                return Ok(entityhci);
            else
                return BadRequest();
        }

        [HttpPut("{hciId}")]
        public async Task<ActionResult<HCIDto>> UpdateHCI(int hciId, UpdateHCIDto hciForUpdate)
        {

            var entityhci = await _supervisorMobilityRepository.GetHCI(hciId);

            var result = await _supervisorMobilityRepository.UpdateHCI(hciForUpdate, entityhci);

            if(result > 0)
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
    }
}
