using AutoMapper;

namespace SupervisorMobility.API.Profiles
{
    public class LupProfile : Profile
    {
        public LupProfile()
        {
            CreateMap<Entities.Lup, Models.LupDtos.LupDto>();
            CreateMap<Entities.Lup, Models.LupDtos.LupForCreationDto>().ReverseMap();
            CreateMap<Entities.Lup, Models.LupDtos.LupForUpdateDto>().ReverseMap();
            CreateMap<Entities.Lup, Models.LupDtos.LupWithoutNavigationPropertiesDto>().ReverseMap();
            CreateMap<Entities.Lup, Models.LupDtos.LupWithFilesDto>();
        }
    }
}
