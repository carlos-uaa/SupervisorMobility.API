
using Microsoft.EntityFrameworkCore;
using SupervisorMobility.API.Context;
using SupervisorMobility.API.DataAccess.Entities;

namespace SupervisorMobility.API.DataAccess.Services.UserCoursesServices
{
    public class UserCoursesRepository : IUserCoursesRepository
    {
        private readonly SupervisorMobilityContext _context;

        public UserCoursesRepository(SupervisorMobilityContext context) {
            _context = context;
        }

        public async Task<ServiceResponse<List<UserCourse>>> GetUserCoursesAsync(string userPayRol)
        {
            try
            {
                var todosLosCursos = _context.UserCourses
                    .FromSqlRaw("EXEC dbo.sp_GetUserCourses")
                    .AsEnumerable();

                var courses = todosLosCursos
                    .Where(uc => uc.PayRol == userPayRol)
                    .ToList();


                if (courses == null || courses.Count == 0)
                    return new ServiceResponse<List<UserCourse>>
                    {
                        Data = null,
                        Success = false,
                        Message = "No courses found for the user."
                    };

                return new ServiceResponse<List<UserCourse>>
                {
                    Data = courses,
                    Success = true,
                    Message = "User courses retrieved successfully."
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<UserCourse>>
                {
                    Data = null,
                    Success = false,
                    Message = $"An error occurred while retrieving user courses: {ex.Message}"
                };
            }
        }
    }
}
