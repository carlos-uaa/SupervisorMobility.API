using AutoMapper;

namespace SupervisorMobility.API.Profiles
{
    public class GlosaryProfile: Profile
    {
        public GlosaryProfile()
        {
            CreateMap<Entities.Glosary, Models.GlosaryDtos.GlosaryDto>();
            CreateMap<Entities.Glosary, Models.GlosaryDtos.GlosaryForCreationDto>().ReverseMap();
            CreateMap<Entities.Glosary, Models.GlosaryDtos.GlosaryForUpdateDto>().ReverseMap();
        }
    }
}
