using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.Models.SOS.SOSCombinationDtos;

namespace SupervisorMobility.API.MapperProfiles.SOSMapperProfiles
{
    public class SOSCombinationProfile : Profile
    {
        public SOSCombinationProfile()
        {
            CreateMap<SOSCombination, SOSCombinationDto>().ReverseMap();
            CreateMap<SOSCombination, SOSCombinationForCreateDto>().ReverseMap();
            CreateMap<SOSCombination, SOSCombinationForUpdateDto>().ReverseMap();
            CreateMap<SOSCombinationForUpdateDto, SOSCombination>().ReverseMap();
        }
    }
}