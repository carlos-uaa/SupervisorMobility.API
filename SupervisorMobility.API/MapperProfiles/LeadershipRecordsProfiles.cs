using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.Models.ILURegisterDtos;

namespace SupervisorMobility.API.MapperProfiles
{
    public class LeadershipRecordsProfiles : Profile
    {
        public LeadershipRecordsProfiles()
        {
            CreateMap<LeadershipRecord, LeadershipRecordsDto>().ReverseMap();
            CreateMap<LeadershipRecord, LeadershipRecordsForCreationDto>().ReverseMap();
            CreateMap<LeadershipRecord, LeadershipRecordsForUpdateDto>().ReverseMap();
            CreateMap<LeadershipRecord, LeadershipRecordsWithoutNavigationDto>().ReverseMap();
        }
    
    }
}
