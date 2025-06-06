using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities.IS;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.CheckpointDtos;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.CheckpointNormDtos;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.DataPanelDtos;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.DataPanelSpecificationDtos;

namespace SupervisorMobility.API.MapperProfiles.ISMapperProfiles
{
    public class CheckpointNormProfile : Profile
    {
        public CheckpointNormProfile()
        {
            //data panel
            CreateMap<Checkpoint, CheckpointDto>().ReverseMap();
            CreateMap<Checkpoint, CheckpointForCreateDto>().ReverseMap();
            CreateMap<Checkpoint, CheckpointForUpdateDto>().ReverseMap().ForMember(dest => dest.Sketches, opt => opt.Ignore()); ;

            CreateMap<CheckpointNorm, CheckpointNormDto>().ReverseMap();
            CreateMap<CheckpointNorm, CheckpointNormForCreateDto>().ReverseMap();
            CreateMap<CheckpointNorm, CheckpointNormForUpdateDto>().ReverseMap().ForMember(dest => dest.Sketches, opt => opt.Ignore()); ;

         
        }
    }
}
