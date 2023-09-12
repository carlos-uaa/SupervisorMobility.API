using AutoMapper;
using SupervisorMobility.API.Models.HeadCount;

namespace SupervisorMobility.API.MapperProfiles
{
    public class HeadCountProfiles : Profile
    {

        public HeadCountProfiles()
        {
            CreateMap<DataAccess.Entities.HeadCount, HeadCountDto>().ReverseMap();
        }

    }
}
