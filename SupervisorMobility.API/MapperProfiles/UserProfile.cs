using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.Users;

namespace SupervisorMobility.API.MapperProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UsersWithNavigationAndPeopleDetails>().ReverseMap();
            CreateMap<User, UsersWithoutNavigationWithoutPeopleDetails>().ReverseMap();
            CreateMap<User, UsersWithPeopleWithoutNavigationDetails>().ReverseMap();
            CreateMap<User, UsersWithoutPeopleWithNavigation>().ReverseMap();
            CreateMap<User, UsersForCreation>().ReverseMap();
            CreateMap<UsersWithNavigationAndPeopleDetails, UsersForCreation>().ReverseMap();
            CreateMap<User, UsersForUpdateDto>().ReverseMap();
            CreateMap<UsersForCreation, UsersWithPeopleWithoutNavigationDetails>().ReverseMap();
            CreateMap<UsersForUpdateDto, UsersWithPeopleWithoutNavigationDetails>().ReverseMap();
            CreateMap<UsersWithoutNavigationWithoutPeopleDetails, UsersForUpdateDto>().ReverseMap();
            CreateMap<UsersWithNavigationAndPeopleDetails, User>().ReverseMap();
        }
    }
}


