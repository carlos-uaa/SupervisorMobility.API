
using Microsoft.EntityFrameworkCore;
using SupervisorMobility.API.Context;

namespace SupervisorMobility.API.DataAccess.Services.LocalUserCourses.Repository
{
    public class LocalUserCoursesRepository : ILocalUserCoursesRepository
    {
        private readonly SupervisorMobilityContext _context;
        public LocalUserCoursesRepository(SupervisorMobilityContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<ServiceResponse<Entities.LocalUserCourses>> UpdateLocalUserCourse(Entities.LocalUserCourses course)
        {
            var response = new ServiceResponse<Entities.LocalUserCourses>();

            try
            {
                var existingCourse = await _context.LocalUserCourses
                    .FirstOrDefaultAsync(c => c.CourseId == course.CourseId);

                if (existingCourse == null)
                {
                    response.Success = false;
                    response.Message = "The course doesn't exist in DB.";
                    return response;
                }

                existingCourse.Reticulate = course.Reticulate;
                existingCourse.Date = course.Date;
                existingCourse.Calification = course.Calification;
                existingCourse.Type = course.Type;

                await _context.SaveChangesAsync();

                response.Data = existingCourse;
                response.Success = true;
                response.Message = "Course correctly updated";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error updating the course: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<Entities.LocalUserCourses>> DeleteLocalUserCourse(int courseId)
        {
            var response = new ServiceResponse<Entities.LocalUserCourses>();

            try
            {
                var existingCourse = await _context.LocalUserCourses
                    .FirstOrDefaultAsync(c => c.CourseId == courseId);

                if (existingCourse == null)
                {
                    response.Success = false;
                    response.Message = "The course doesn't exist in DB.";
                    return response;
                }

                _context.LocalUserCourses.Remove(existingCourse);
                await _context.SaveChangesAsync();

                response.Data = existingCourse;
                response.Success = true;
                response.Message = "Course correctly deleted!.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error deleting the course: {ex.Message}";
            }

            return response;

        }
    }
}
