using AutoMapper;

namespace SupervisorMobility.API.Profiles
{
    public class PlantProfile : Profile
    {
        public PlantProfile()
        {
            CreateMap<Entities.Plant, Models.PlantDtos.PlantDto>().ReverseMap();
            CreateMap<Entities.Plant, Models.PlantDtos.PlantWithJustAreasDto>().ReverseMap();
            CreateMap<Entities.Plant, Models.PlantDtos.PlantForCreationDto>().ReverseMap();
            CreateMap<Entities.Plant, Models.PlantDtos.PlantForUpdateDto>().ReverseMap();
            CreateMap<Entities.Plant, Models.PlantDtos.GetPlantForHRIDto>().ReverseMap();
        }
    }
}
