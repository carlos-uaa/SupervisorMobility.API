using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.Models.SOS.SOSHubDtos;

namespace SupervisorMobility.API.MapperProfiles.ISOSMapperProfiles
{
    public class SOSDataHubProfile : Profile
    {
        public SOSDataHubProfile()
        {
            CreateMap<SOSHub, SOSHubDto>().ReverseMap();
            CreateMap<SOSHub, SOSHubForCreateDto>().ReverseMap();
            CreateMap<SOSHubForUpdateDto, SOSHubForCreateDto>().ReverseMap();
            CreateMap<SOSHub, SOSHubForUpdateDto>().ReverseMap();
        }
    }
}