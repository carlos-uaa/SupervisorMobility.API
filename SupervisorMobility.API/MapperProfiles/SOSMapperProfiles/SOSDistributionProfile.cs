using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.Models.SOS.SOSDistributionDtos;

namespace SupervisorMobility.API.MapperProfiles.SOSMapperProfiles
{
    public class SOSDistributionProfile : Profile
    {
        public SOSDistributionProfile()
        {
            CreateMap<SOSDistribution, SOSDistributionDto>().ReverseMap();
            CreateMap<SOSDistribution, SOSDistributionForCreateDto>().ReverseMap();
            CreateMap<SOSDistribution, SOSDistributionForUpdateDto>().ReverseMap();
        }
    }
}