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
            CreateMap<Entities.Lup, Models.LupDtos.LupWithFilesDto>().ReverseMap();
            CreateMap<Entities.Findings, Models.LupDtos.FindingsDto>().ReverseMap();
            CreateMap<Models.LupDtos.FindingsDto, Entities.Findings>().ReverseMap();
            CreateMap<Entities.Findings, Models.LupDtos.FindingsForCreateDto>().ReverseMap();
            CreateMap<Entities.Findings, Entities.Findings>().ReverseMap();
        }
    }
}
