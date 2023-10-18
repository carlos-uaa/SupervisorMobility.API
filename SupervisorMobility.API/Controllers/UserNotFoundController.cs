using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.Users;
using SupervisorMobility.API.Models.ReturnResults;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Drawing.Text;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using DocumentFormat.OpenXml.Wordprocessing;
using SupervisorMobility.API.DataAccess.Services;
using SupervisorMobility.API.Services;

namespace SupervisorMobility.API.Controllers
{
    [Route("api/UserNotFound")]
    [ApiController]
    public class UserNotFoundController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        private readonly IAssyChartService _assyChartService;
        private readonly Services.ISupervisorMobilityRepository _supervisorMobilityRepository;
        private readonly IEmailService _email;

        public UserNotFoundController(IWebHostEnvironment env, ISupervisorMobilityRepository supervisorMobilityRepository,
            IMapper mapper, IAssyChartService assyChartService, IEmailService emailService)
        {
            _email = emailService;
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

            var emailMessage = _email.CreateEmailMessage("juan.herreraespinoza@compas-mx.com", $"This user tried to login: {finalUser.ObjectId} with this Name: {finalUser.Name}");
            _email.Send(emailMessage);

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

