using AutoMapper;

namespace SupervisorMobility.API.MapperProfiles
{
    public class PATProfile : Profile
    {
        public PATProfile() {
            CreateMap<DataAccess.Entities.PAT, Models.PATDtos.PATDto>().ReverseMap();
            CreateMap<DataAccess.Entities.PAT, Models.PATDtos.PATForUpdateDto>().ReverseMap();
            CreateMap<DataAccess.Entities.PAT, Models.PATDtos.PATFotCreationDto>().ReverseMap();
            CreateMap<DataAccess.Entities.PAT, Models.PATDtos.PATwithoutNavigations>().ReverseMap();
        }
    }

   
}
