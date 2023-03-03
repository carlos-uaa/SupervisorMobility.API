using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.Users;

namespace SupervisorMobility.API.MapperProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile() { 
            CreateMap<User, UsersDataToBulk>().ReverseMap();
            CreateMap<User, UsersWhitNavigationDetails>().ReverseMap();
            CreateMap<User, UsersWhitoutNavigationDetails>().ReverseMap();
            CreateMap<User, UsersForCreation>().ReverseMap();
            CreateMap<User, UsersForUpdateDto>().ReverseMap();
        }
    }
}


