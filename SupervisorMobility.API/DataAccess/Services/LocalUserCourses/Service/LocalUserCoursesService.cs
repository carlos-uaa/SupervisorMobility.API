
using SupervisorMobility.API.DataAccess.Services.LocalUserCourses.Repository;

namespace SupervisorMobility.API.DataAccess.Services.LocalUserCourses.Service
{
    public class LocalUserCoursesService : ILocalUserCoursesService
    {
        private readonly ILocalUserCoursesRepository _localUserCoursesRepository;
        public LocalUserCoursesService(ILocalUserCoursesRepository localUserCoursesRepository)
        {
            _localUserCoursesRepository = localUserCoursesRepository;
        }

        public async Task<ServiceResponse<Entities.LocalUserCourses>> UpdateLocalUserCourse(Entities.LocalUserCourses course)
        {
            var response = new ServiceResponse<Entities.LocalUserCourses>();

            // Validación básica antes de ir al repositorio
            if (course == null || course.CourseId <= 0)
            {
                response.Success = false;
                response.Message = "The course you sent is not valid.";
                return response;
            }

            if (string.IsNullOrWhiteSpace(course.Reticulate))
            {
                response.Success = false;
                response.Message = "The field Retículate is required.";
                return response;
            }

            if ((course.Date == default))
            {
                response.Success = false;
                response.Message = "The field Date is required.";
                return response;
            }

            if (course.Calification <= 0)
            {
                response.Success = false;
                response.Message = "The field Calification is required.";
                return response;
            }

            if (string.IsNullOrWhiteSpace(course.Type))
            {
                response.Success = false;
                response.Message = "The field Type is required.";
                return response;
            }

            response = await _localUserCoursesRepository.UpdateLocalUserCourse(course);

            return response;
        }

        public async Task<ServiceResponse<Entities.LocalUserCourses>> DeleteLocalUserCourse(int courseId)
        {
            var response = new ServiceResponse<Entities.LocalUserCourses>();

            if (courseId <= 0)
            {
                response.Success = false;
                response.Message = "El identificador del curso no es válido.";
                return response;
            }

            response = await _localUserCoursesRepository.DeleteLocalUserCourse(courseId);

            return response;
        }
    }
}
