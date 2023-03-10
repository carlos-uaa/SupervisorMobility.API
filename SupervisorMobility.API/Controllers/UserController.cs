using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.Business;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.FileUpload;
using SupervisorMobility.API.Models.FileUploadDto;
using SupervisorMobility.API.Models.Users;
using SupervisorMobility.API.Services;
using System.Net;
using System.Text.RegularExpressions;

namespace SupervisorMobility.API.Controllers
{
    [Route("api/Users")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        private readonly IAssyChartService _assyChartService;
        private readonly ISupervisorMobilityRepository _supervisorMobilityRepository;

        public UsersController(IWebHostEnvironment env, ISupervisorMobilityRepository supervisorMobilityRepository,
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
        public async Task<ActionResult<IEnumerable<UsersWhitNavigationDetails>>> GetUsers(bool collections = false)
        {
            if (collections)
            {
                var userEntity = await _supervisorMobilityRepository.GetAllUsersWhitPlantAreaAndGroupAsync();
                return Ok(_mapper.Map<IEnumerable<UsersWhitNavigationDetails>>(userEntity));
            }
            else
            {
                var userEntity = await _supervisorMobilityRepository.GetAllUsersAsync();
                return Ok(_mapper.Map<IEnumerable<UsersWhitoutNavigationDetails>>(userEntity));
            }
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<UsersWhitNavigationDetails>> GetUser(int userId,bool collections = false)
        {
            if (collections)
            {
                var userEntity = await _supervisorMobilityRepository.GetUserAsync(userId, collections);
                if (userEntity != null)
                {
                    return Ok(_mapper.Map<UsersWhitNavigationDetails>(userEntity));

                }

                return NotFound();
            }
            else
            {
                var userEntity = await _supervisorMobilityRepository.GetUserAsync(userId);
                if (userEntity != null)
                {
                    return Ok(_mapper.Map<UsersWhitoutNavigationDetails>(userEntity));

                }

                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult<UsersWhitNavigationDetails>> CreateUser(UsersForCreation newUser)
        {
            if (!await _supervisorMobilityRepository.PlantExistAsync(newUser.PlantId))
            {
                return NotFound("No Planta");
            }

            if (!await _supervisorMobilityRepository.AreaExistAsync(newUser.AreaId))
            {
                return NotFound("No Area");
            }

            if (!await _supervisorMobilityRepository.GroupExistAsync(newUser.GroupId))
            {
                return NotFound("No Group");
            }

            var finalUser = await _assyChartService.CreateUserAsync(newUser);

            return Ok(finalUser);

        }

        [HttpPut("{userId}")]
        public async Task<ActionResult> UpdateUser(int userId, UsersForUpdateDto user)
        {
            var userEntity = await _assyChartService.FetchUserAsync(userId);
            if (userEntity == null)
            {
                return NotFound();
            }

            await _assyChartService.UpdateUserAsync(user, userEntity);

            return Ok();
        }

        [HttpDelete("{userId}")]
        public async Task<ActionResult> DeleteUser(int userId)
        {
            var userEntity = await _assyChartService.FetchUserAsync(userId);
            if (userEntity == null)
            {
                return NotFound();
            }

            await _assyChartService.RemoveUserAsync(userEntity);

            return Ok();
        }

        //******* File users    **********//
        [HttpPost("FileUpload")]
        public async Task<ActionResult<FileUploadGeneralDto>> UploadUsersFile(IFormFile file)
        {
            FileUploadGeneralDto uploadResult = new FileUploadGeneralDto();
            string trustedFileNameForFileStorage;
            var untrustedFileName = file.FileName;
            uploadResult.FileName = untrustedFileName;

            var trsutedFileNameForDisplay = WebUtility.HtmlEncode(untrustedFileName);


            Regex regexcsv = new Regex(".+\\.csv", RegexOptions.Compiled);
            Regex regexlsx = new Regex(".+\\.xlsx", RegexOptions.Compiled);

            if (regexcsv.IsMatch(untrustedFileName))
                trustedFileNameForFileStorage = Path.ChangeExtension(Path.GetRandomFileName(), "csv");
            else if (regexlsx.IsMatch(untrustedFileName))
                trustedFileNameForFileStorage = Path.ChangeExtension(Path.GetRandomFileName(), "xlsx");
            else
                trustedFileNameForFileStorage = Path.GetRandomFileName();

            //trustedFileNameForFileStorage = Path.GetRandomFileName();
            var path = Path.Combine(_env.ContentRootPath, "uploads\\users", trustedFileNameForFileStorage);

            await using FileStream fs = new(path, FileMode.Create);
            await file.CopyToAsync(fs);

            uploadResult.StorageFileName = trustedFileNameForFileStorage;

            return Ok(uploadResult);
        }

        [HttpPost("FileUpload/Data")]
        //public async Task<ActionResult<string>> ApplyUsersUpload(UploadResult FileInfo)
        public async  Task<ActionResult> ApplyUsersUpload(FileUploadGeneralDto FileInfo)
        {
            string file = Directory.GetCurrentDirectory().ToString() + "\\uploads\\users\\" + FileInfo.StorageFileName;
            
            
            return Ok();

        }


    }
}