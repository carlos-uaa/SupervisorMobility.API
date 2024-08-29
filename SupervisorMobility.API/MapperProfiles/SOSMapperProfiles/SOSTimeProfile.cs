using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.Models.SOS.SOSTimeDtos;

namespace SupervisorMobility.API.MapperProfiles.SOSMapperProfiles
{
    public class SOSTimeProfile : Profile
    {
        public SOSTimeProfile()
        {
            CreateMap<SOSTime, SOSTimeDto>().ReverseMap();
            CreateMap<SOSTime, SOSTimeForCreateDto>().ReverseMap();
            CreateMap<SOSTime, SOSTimeForUpdateDto>().ReverseMap();
        }
    }
}