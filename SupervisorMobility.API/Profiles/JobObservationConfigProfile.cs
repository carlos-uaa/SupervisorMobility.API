using AutoMapper;

namespace SupervisorMobility.API.Profiles
{
    public class JobObservationConfigProfile : Profile
    {
        public JobObservationConfigProfile()
        {
            CreateMap<Entities.JobObservationConfig, Models.JobObservationConfigsDtos.JobObservationConfigsWithoutNavigationPropertiesDto>().ReverseMap();
            CreateMap<Models.JobObservationConfigsDtos.JobObservationConfigForCreationDto, Entities.JobObservationConfig>();
            CreateMap<Entities.JobObservationConfig, Models.JobObservationConfigsDtos.JobObservationConfigForUpdateDto>().ReverseMap();
        }
    }
}
