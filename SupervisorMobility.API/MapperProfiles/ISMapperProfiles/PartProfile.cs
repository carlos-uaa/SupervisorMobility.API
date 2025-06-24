using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities.IS;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.DataPanelDtos;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.DataPanelSpecificationDtos;
using SupervisorMobility.API.Models.IS_Apariencia_PlantillaDtos.PartDtos;

namespace SupervisorMobility.API.MapperProfiles.ISMapperProfiles
{
    public class PartProfile : Profile
    {
        public PartProfile()
        {
            //data panel
            CreateMap<Part, PartDto>().ReverseMap();
            CreateMap<Part, PartForCreateDto>().ReverseMap();
            CreateMap<Part, PartForUpdateDto>().ReverseMap()
                .ForMember(dest=>dest.Sketches, opt => opt.Ignore());

         
        }
    }
}
