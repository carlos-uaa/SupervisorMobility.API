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
            
            // NOTE: Map DTO to Entity but ignore SOSHubs (EF Core will handle relationship separately)
            CreateMap<SOSSynopticTableofOperatingRequirementsForCreateDto, SOSSynopticTableofOperatingRequirements>()
                .ForMember(dest => dest.SOSHubs, opt => opt.Ignore());
            
            // NOTE: Reverse map (Entity to DTO) includes SOSHubs
            CreateMap<SOSSynopticTableofOperatingRequirements, SOSSynopticTableofOperatingRequirementsForCreateDto>();
            
            CreateMap<SOSSynopticTableofOperatingRequirements, SOSSynopticRequirementsForUpdateDto>().ReverseMap();
        }
    }
}