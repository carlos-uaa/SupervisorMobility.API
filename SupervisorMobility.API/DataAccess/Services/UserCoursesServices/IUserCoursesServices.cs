using SupervisorMobility.API.DataAccess.Entities;

namespace SupervisorMobility.API.DataAccess.Services.UserCoursesServices
{
    public interface IUserCoursesServices
    {
        Task<ServiceResponse<List<UserCourse>>> GetUserCoursesAsync(string userPayRol);
    }
}
