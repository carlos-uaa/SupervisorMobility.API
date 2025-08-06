using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.Models.SOS.SOSSynopticTableofOperatingRequirementsDtos;

namespace SupervisorMobility.API.MapperProfiles.SOSMapperProfiles
{
    public class SOSSynopticRequirementsProfile : Profile
    {
        public SOSSynopticRequirementsProfile()
        {
            CreateMap<SOSSynopticTableofOperatingRequirements, SOSSynopticRequirementsDto>().ReverseMap();
            CreateMap<SOSSynopticTableofOperatingRequirements, SOSSynopticTableofOperatingRequirementsForCreateDto>().ReverseMap();
            CreateMap<SOSSynopticTableofOperatingRequirements, SOSSynopticRequirementsForUpdateDto>().ReverseMap();
        }
    }
}