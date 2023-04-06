using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities;

namespace SupervisorMobility.API.Profiles
{
    public class JobObservation : Profile
    {
        public JobObservation()
        {
            CreateMap<Entities.JobObservation, Models.JobObservationDtos.JobObservationDto>();
            CreateMap<Entities.JobObservation, Models.JobObservationDtos.JobObservationForCreationDto>().ReverseMap();
            CreateMap<Entities.JobObservation, Models.JobObservationDtos.JobObservationForUpdateDto>().ReverseMap();
            CreateMap<Entities.JobObservation, Models.JobObservationDtos.JobObservationWithoutNavigationPropertiesDto>().ReverseMap();
            CreateMap<Entities.JobObservation, Models.JobObservationDtos.JobObservationWithJustLupDto>().ReverseMap();
            CreateMap<JobObservationVersion, Entities.JobObservation>().ReverseMap();
            CreateMap<JobObservationVersion, Models.JobObservationDtos.JobObservationHistoryDto>().ReverseMap();
        }
    }
}
