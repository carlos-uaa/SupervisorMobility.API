using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.Models.SOS.ToolDtos;
using SupervisorMobility.API.Models.SOS.ToolsUsedDtos;

namespace SupervisorMobility.API.MapperProfiles.SOSMapperProfiles
{
    public class ToolUsedProfile : Profile
    {
        public ToolUsedProfile()
        {
            CreateMap<ToolUsed, ToolUsedDto>().ReverseMap();
            CreateMap<ToolUsed, ToolUsedForCreateDto>().ReverseMap();
            CreateMap<ToolUsed, ToolUsedForUpdateDto>().ReverseMap();
        }
    }
}