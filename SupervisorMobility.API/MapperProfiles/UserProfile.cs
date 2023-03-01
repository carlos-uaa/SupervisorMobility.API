using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.Users;

namespace SupervisorMobility.API.MapperProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile() { 
            CreateMap<User, UsersDataToBulk>();
            CreateMap<User, UsersWhitNavigationDetails>();
            CreateMap<User, UsersForCreation>();
            CreateMap<User, UsersForUpdateDto>();
        }
    }
}


