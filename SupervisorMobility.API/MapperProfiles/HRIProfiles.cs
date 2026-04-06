using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities.HRI;
using SupervisorMobility.API.Models.HRIDtos;

namespace SupervisorMobility.API.MapperProfiles
{
    public class HRIProfiles : Profile
    {
        public HRIProfiles()
        {
               CreateMap<HRI,GetHRIDto>().ReverseMap();
                CreateMap<CreateHRIDto,HRI>().ReverseMap();
        }
    }
}
