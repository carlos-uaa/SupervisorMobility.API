using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.Models.SOS.SOSFlowDtos;

namespace SupervisorMobility.API.MapperProfiles.SOSMapperProfiles
{
    public class SOSFlowProfile : Profile
    {
        public SOSFlowProfile()
        {
            CreateMap<SOSFlow, SOSFlowDto>().ReverseMap();
            CreateMap<SOSFlow, SOSFlowForCreateDto>().ReverseMap();
            CreateMap<SOSFlow, SOSFlowForUpdateDto>().ReverseMap();
        }
    }
}