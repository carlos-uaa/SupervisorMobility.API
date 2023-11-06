using AutoMapper;

namespace SupervisorMobility.API.Profiles
{
    public class PillarProfile : Profile
    {
        public PillarProfile()
        {
            CreateMap<Entities.Pillar, Models.PillarDtos.PillarDto>();
            CreateMap<Entities.Pillar, Models.PillarDtos.PillarForCreationDto>().ReverseMap();
            CreateMap<Entities.Pillar, Models.PillarDtos.PillarForUpdateDto>().ReverseMap();
        }
    }
}
