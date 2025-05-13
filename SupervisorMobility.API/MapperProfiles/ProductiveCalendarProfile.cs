using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.ILU;
using SupervisorMobility.API.DataAccess.Entities.LUP;

namespace SupervisorMobility.API.MapperProfiles
{
    public class ProductiveCalendarProfile : Profile
    {
        public ProductiveCalendarProfile()
        {
            CreateMap<Holiday, Models.ProductiveCalendarDtos.HolidayForUpdateDto>().ReverseMap();
        }
    }


}
