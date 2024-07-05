using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.IS;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.AppearanceDtos;

namespace SupervisorMobility.API.MapperProfiles.ISMapperProfiles
{
    public class AppearanceProfile : Profile
    {
        public AppearanceProfile()
        {
            //data panel
            CreateMap<Appearance, AppearanceDto>();
            CreateMap<Appearance, AppearanceForCreateDto>().ReverseMap();
            CreateMap<Appearance, AppearanceForUpdateDto>().ReverseMap();
            CreateMap<Commentary, Models.CommentaryDtos.CommentaryDto>().ReverseMap();
            CreateMap<Commentary, Models.CommentaryDtos.CreateCommentaryDto>().ReverseMap();
            CreateMap<Commentary, Models.CommentaryDtos.UpdateCommentaryDto>().ReverseMap();
        }
    }
}
