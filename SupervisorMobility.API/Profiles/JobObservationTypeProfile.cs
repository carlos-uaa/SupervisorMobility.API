using AutoMapper;

namespace SupervisorMobility.API.Profiles
{
    public class JobObservationTypeProfile : Profile
    {
        public JobObservationTypeProfile()
        {
            CreateMap<Entities.JobObservationType, Models.JobObservationTypeDtos.JobObservationTypeWithoutConfigsDto>();
            CreateMap<Entities.JobObservationType, Models.JobObservationTypeDtos.JobObservationTypeForCreationDto>().ReverseMap();
            CreateMap<Entities.JobObservationType, Models.JobObservationTypeDtos.JobObservationTypeDto>().ReverseMap();
            CreateMap<Entities.JobObservationType, Models.JobObservationTypeDtos.JobObservationTypeForUpdateDto>().ReverseMap();
        }
    }
}
