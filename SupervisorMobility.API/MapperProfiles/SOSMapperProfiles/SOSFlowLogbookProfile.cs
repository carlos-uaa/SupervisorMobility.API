using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.Models.SOS.SOSFlowDtos;
using SupervisorMobility.API.Models.SOS.SOSFlowLogbookDtos;

namespace SupervisorMobility.API.MapperProfiles.SOSMapperProfiles
{
    public class SOSFlowLogbookProfile : Profile
    {
        public SOSFlowLogbookProfile()
        {
            CreateMap<SOSFlowLogbook, SOSFlowLogbookDto>().ReverseMap();
            CreateMap<SOSFlowLogbook, SOSFlowLogbookForCreateDto>().ReverseMap();
            CreateMap<SOSFlowLogbook, SOSFlowLogbookForUpdateDto>().ReverseMap();
        }
    }
}