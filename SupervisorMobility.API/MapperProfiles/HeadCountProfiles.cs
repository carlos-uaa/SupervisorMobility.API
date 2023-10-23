using AutoMapper;
using SupervisorMobility.API.Models.HeadCount;

namespace SupervisorMobility.API.MapperProfiles
{
    public class HeadCountProfiles : Profile
    {

        public HeadCountProfiles()
        {
            CreateMap<DataAccess.Entities.HeadCount, HeadCountDto>().ReverseMap();
            CreateMap<DataAccess.Entities.HeadCountProcess, HeadCountProcessDto>().ReverseMap();
            CreateMap<DataAccess.Entities.HeadCountProcess, HeadCountProcessCreateUpdateDto>().ReverseMap();
        }

    }
}
