using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Services.UserCoursesServices;

namespace SupervisorMobility.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserCoursesController : Controller
    {
        private readonly IUserCoursesServices _userCoursesService;
        public UserCoursesController(IUserCoursesServices userCoursesService)
        {
            _userCoursesService = userCoursesService;
        }

        [HttpGet("GetUserCoursesAsync/{payrol}")]
        public async Task<ActionResult<ServiceResponse<List<UserCourse>>>> GetUserCoursesAsync(string payrol)
        {
            var response = await _userCoursesService.GetUserCoursesAsync(payrol);
            if (response.Data == null)
                return NotFound(response);
            return StatusCode(200, response);
        }
    }
}
