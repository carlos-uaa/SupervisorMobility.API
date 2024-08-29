using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.Models.SOS.SOSCombinationDtos;
using SupervisorMobility.API.Models.SOS.SOSCombinationLogbookDtos;

namespace SupervisorMobility.API.MapperProfiles.SOSMapperProfiles
{
    public class SOSCombinationLogbookProfile : Profile
    {
        public SOSCombinationLogbookProfile()
        {
            CreateMap<SOSCombinationLogbook, SOSCombinationLogbookDto>().ReverseMap();
            CreateMap<SOSCombinationLogbook, SOSCombinationLogbookForCreateDto>().ReverseMap();
            CreateMap<SOSCombinationLogbook, SOSCombinationLogbookForUpdateDto>().ReverseMap();
        }
    }
}