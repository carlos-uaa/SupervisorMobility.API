using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.Models.SOS.ToolDtos;

namespace SupervisorMobility.API.MapperProfiles.ISOSMapperProfiles
{
    public class ToolProfile : Profile
    {
        public ToolProfile()
        {
            CreateMap<Tool, ToolDto>().ReverseMap();
            CreateMap<Tool, ToolForCreateDto>().ReverseMap();
            CreateMap<Tool, ToolForUpdateDto>().ReverseMap();
        }
    }
}