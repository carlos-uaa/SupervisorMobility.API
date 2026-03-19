namespace SupervisorMobility.API.DataAccess.Services.LocalUserCourses.Service
{
    public interface ILocalUserCoursesService
    {
        Task<ServiceResponse<Entities.LocalUserCourses>> UpdateLocalUserCourse(Entities.LocalUserCourses course);
        Task<ServiceResponse<Entities.LocalUserCourses>> DeleteLocalUserCourse(int courseId);
    }
}
