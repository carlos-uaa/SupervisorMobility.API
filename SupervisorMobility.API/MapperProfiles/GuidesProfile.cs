using AutoMapper;

namespace SupervisorMobility.API.MapperProfiles
{
    public class GuidesProfile : Profile
    {

        public GuidesProfile()
        {
            CreateMap<DataAccess.Entities.Guides, Models.GuidesDtos.GuideWithFileInfoDto>();
            CreateMap<DataAccess.Entities.Guides, Models.GuidesDtos.GuideWithoutFileDto>().ReverseMap();
            CreateMap<DataAccess.Entities.Guides, Models.GuidesDtos.GuideForCreationDto>().ReverseMap();
            CreateMap<DataAccess.Entities.Guides, Models.GuidesDtos.GuideForUpdateDto>().ReverseMap();
        }

    }
}
