using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic.FileIO;
using SpreadsheetLight;
using SupervisorMobility.API.Entities;
using SupervisorMobility.API.Models.AreaDtos;
using SupervisorMobility.API.Models.AssyChart;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using SupervisorMobility.API.Services;
using SupervisorMobility.API.Models.PlantDtos;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.DataAccess.Entities;
using Microsoft.AspNetCore.Cors;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.Users;
using SupervisorMobility.API.Models.ReturnResults;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Drawing.Text;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using DocumentFormat.OpenXml.Wordprocessing;

namespace SupervisorMobility.API.Controllers
{
    [Route("api/UserNotFound")]
    [ApiController]
    public class UserNotFoundController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        private readonly IAssyChartService _assyChartService;
        private readonly ISupervisorMobilityRepository _supervisorMobilityRepository;

        public UserNotFoundController(IWebHostEnvironment env, ISupervisorMobilityRepository supervisorMobilityRepository,
            IMapper mapper, IAssyChartService assyChartService)
        {
            _env = env;
            _supervisorMobilityRepository = supervisorMobilityRepository ??
                throw new ArgumentNullException(nameof(supervisorMobilityRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
            _assyChartService = assyChartService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserNotFoundWithNavigationDetails>>> GetUsersNotFound()
        {
            var userNotFoundEntity = await _supervisorMobilityRepository.GetAllUsersNotFoundAsync();

            return Ok(_mapper.Map<IEnumerable<UserNotFoundWithNavigationDetails>>(userNotFoundEntity));
        }


        [HttpPost]
        public async Task<ActionResult<UserNotFoundWithNavigationDetails>> CreateUserNotFound(UserNotFoundForCreation newUserNotFound)
        {
            var finalUser = await _assyChartService.CreateUserNotFoundAsync(newUserNotFound);

            var UserToReturn = _mapper.Map<UserNotFound>(finalUser);
            await _supervisorMobilityRepository.SaveChangesAsync();

            return Ok(UserToReturn);
        }

        [HttpPut("{userNotFoundId}")]
        public async Task<ActionResult> UpdateUserNotFound(int userNotFoundId, UserNotFoundForUpdateDto userNotFound)
        {

            var userNotFoundEntity = await _assyChartService.FetchUserNotFoundAsync(userNotFoundId);
            if (userNotFoundEntity == null)
            {
                return NotFound();
            }

            await _assyChartService.UpdateUserNotFoundAsync(userNotFound, userNotFoundId);

            return Ok();
        }
    }
}

