using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.Models.SOS.TurnDtos;

namespace SupervisorMobility.API.MapperProfiles.SOSMapperProfiles
{
    public class TurnProfile : Profile
    {
        public TurnProfile()
        {
            CreateMap<Turn, TurnDto>().ReverseMap();
            CreateMap<Turn, TurnForCreateDto>().ReverseMap();
            CreateMap<Turn, TurnForUpdateDto>().ReverseMap();
        }
    }
}