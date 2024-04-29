using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.HCIDtos;
using SupervisorMobility.API.Services;
using System.Diagnostics;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Services.TreeServices;
using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.AreaDtos;
using Microsoft.IdentityModel.Tokens;
using SupervisorMobility.API.DataAccess.Entities.Paths;
using FuzzyString;
using SupervisorMobility.API.Models.AssyChart;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
using SupervisorMobility.API.Models.OperationDtos;
using Slugify;
using SupervisorMobility.API.Models.DistributionDtos;
using SupervisorMobility.API.DataAccess.Entities.TreeStruct;
using SupervisorMobility.API.Entities.CDMS;
using SupervisorMobility.API.Context;
using DocumentFormat.OpenXml.Bibliography;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.EMMA;
using Irony.Parsing;
using DuoVia.FuzzyStrings;
using SupervisorMobility.API.Models.HCICategoryDtos;

namespace SupervisorMobility.API.Controllers
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

            var entityhci = await _supervisorMobilityRepository.AddHCI(hciEntity);

            if (entityhci != null)
                return Ok(_mapper.Map<HCIDto>(hciEntity));
            else
                return BadRequest();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HCIDto>>> GetAllDataHCI(bool includeNavigation = false, bool includePeople = false, bool includeComments = false, bool includeTransactions = false)
        {

            var entityhci = await _supervisorMobilityRepository.GetAllHCIs(includeNavigation, includePeople, includeComments, includeTransactions);
            if (entityhci != null)
                return Ok(entityhci);
            else
                return BadRequest(); 
        }

        [HttpGet("{hciId}")]
        public async Task<ActionResult<HCIDto>> GetHCI(int hciId, bool includeNavigation = false, bool includePeople = false, bool includeComments = false, bool includeTransactions = false)
        {

            var entityhci = await _supervisorMobilityRepository.GetHCI(hciId, includeNavigation, includePeople, includeComments, includeTransactions);
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

        [HttpGet("Categories")]
        public async Task<ActionResult<IEnumerable<HCICategoryDto>>> GetHCICategories()
        {
            var resultlist = await _supervisorMobilityRepository.GetHCICategories();
            if(resultlist != null)
                return Ok(resultlist);
            else
                return BadRequest();
        }
    }
}
