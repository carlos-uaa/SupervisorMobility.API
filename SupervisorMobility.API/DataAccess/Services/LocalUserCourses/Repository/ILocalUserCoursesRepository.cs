namespace SupervisorMobility.API.DataAccess.Services.LocalUserCourses.Repository
{
    public interface ILocalUserCoursesRepository
    {
        Task<ServiceResponse<Entities.LocalUserCourses>> UpdateLocalUserCourse(Entities.LocalUserCourses course);
        Task<ServiceResponse<Entities.LocalUserCourses>> DeleteLocalUserCourse(int courseId);
    }
}
