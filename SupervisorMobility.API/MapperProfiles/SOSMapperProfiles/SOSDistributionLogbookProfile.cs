using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.Models.SOS.SOSDistributionDtos;
using SupervisorMobility.API.Models.SOS.SOSDistributionLogbookDtos;

namespace SupervisorMobility.API.MapperProfiles.SOSMapperProfiles
{
    public class SOSDistributionLogbookProfile : Profile
    {
        public SOSDistributionLogbookProfile()
        {
            CreateMap<SOSDistributionLogbook, SOSDistributionLogbookDto>().ReverseMap();
            CreateMap<SOSDistributionLogbook, SOSDistributionLogbookForCreateDto>().ReverseMap();
            CreateMap<SOSDistributionLogbook, SOSDistributionLogbookForUpdateDto>().ReverseMap();
        }
    }
}