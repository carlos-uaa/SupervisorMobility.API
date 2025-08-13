using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.Models.SOS.SOSSynopticTableofControlPointsDtos;

namespace SupervisorMobility.API.MapperProfiles.SOSMapperProfiles
{
    public class SOSSynopticControlPointsProfile : Profile
    {
        public SOSSynopticControlPointsProfile()
        {
            CreateMap<SOSSynopticTableofControlPoints, SOSSynopticControlPointsDto>().ReverseMap();
            CreateMap<SOSSynopticTableofControlPoints, SOSSynopticTableofControlPointsForCreateDto>().ReverseMap();
        }
    }
}