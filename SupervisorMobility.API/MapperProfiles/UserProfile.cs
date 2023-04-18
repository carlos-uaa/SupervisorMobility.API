using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.Users;

namespace SupervisorMobility.API.MapperProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile() { 
            CreateMap<User, UsersWithNavigationDetails>().ReverseMap();
            CreateMap<User, UsersWithoutNavigationDetails>().ReverseMap();
            CreateMap<User, UsersForCreation>().ReverseMap();
            CreateMap<User, UsersForUpdateDto>().ReverseMap();
        }
    }
}


