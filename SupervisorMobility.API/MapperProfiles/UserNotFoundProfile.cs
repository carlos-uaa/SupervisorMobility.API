using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.Users;

namespace SupervisorMobility.API.MapperProfiles
{
    public class UserNotFoundProfile : Profile
    {
        public UserNotFoundProfile()
        {
            CreateMap<UserNotFound, UserNotFoundWithNavigationDetails>().ReverseMap();
            CreateMap<UserNotFound, UserNotFoundForCreation>().ReverseMap();
            CreateMap<UserNotFound, UserNotFoundForUpdateDto>().ReverseMap();
        }
    }
}


