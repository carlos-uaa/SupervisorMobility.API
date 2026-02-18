

using SupervisorMobility.API.DataAccess.Entities;

namespace SupervisorMobility.API.DataAccess.Services.UserCoursesServices
{
    public class UserCoursesServices : IUserCoursesServices
    {
        private readonly IUserCoursesRepository _repository;
        public UserCoursesServices(IUserCoursesRepository repository)
        {
            _repository = repository;
        }

        public async Task<ServiceResponse<List<UserCourse>>> GetUserCoursesAsync(string userPayRol)
        {
            try
            {
                if (string.IsNullOrEmpty(userPayRol))
                    return new ServiceResponse<List<UserCourse>>
                    {
                        Data = null,
                        Success = false,
                        Message = "User PayRol cannot be null or empty."
                    };

                var response = await _repository.GetUserCoursesAsync(userPayRol);
                if (!response.Success)
                    return new ServiceResponse<List<UserCourse>>
                    {
                        Data = null,
                        Success = false,
                        Message = response.Message
                    };

                return new ServiceResponse<List<UserCourse>>
                {
                    Data = response.Data,
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
