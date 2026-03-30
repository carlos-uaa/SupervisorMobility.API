using Microsoft.AspNetCore.Mvc;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Services.LocalUserCourses.Service;

namespace SupervisorMobility.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocalUserCoursesController : ControllerBase
    {
        private readonly ILocalUserCoursesService _localUserCoursesService;
        public LocalUserCoursesController(ILocalUserCoursesService localUserCoursesService)
        {
            _localUserCoursesService = localUserCoursesService;
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateLocalUserCourse([FromBody] LocalUserCourses course)
        {
            if (course == null)
                return BadRequest("The course sent is invalid.");

            var result = await _localUserCoursesService.UpdateLocalUserCourse(course);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpDelete("delete/{courseId}")]
        public async Task<IActionResult> DeleteLocalUserCourse(int courseId)
        {
            if (courseId <= 0)
                return BadRequest("The course identifier sent is invalid.");

            var result = await _localUserCoursesService.DeleteLocalUserCourse(courseId);

            if (!result.Success)
                return NotFound(result.Message);

            return Ok(result);
        }

    }
}
