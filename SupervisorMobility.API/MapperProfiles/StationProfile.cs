using AutoMapper;

namespace SupervisorMobility.API.Profiles
{
    public class StationProfile : Profile
    {
        public StationProfile()
        {
            CreateMap<Entities.Station, Models.StationDtos.StationDto>();
            CreateMap<Entities.Station, Models.StationDtos.StationForCreationDto>().ReverseMap();
            CreateMap<Entities.Station, Models.StationDtos.StationForUpdateDto>().ReverseMap();
        }
    }
}
