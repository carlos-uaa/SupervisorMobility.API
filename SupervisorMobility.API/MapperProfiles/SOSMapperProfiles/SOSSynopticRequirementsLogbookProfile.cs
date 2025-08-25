using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.Models.SOS.SOSSynopticTableofOperatingRequirementsLogbookDtos;

namespace SupervisorMobility.API.MapperProfiles.SOSMapperProfiles
{
    public class SOSSynopticRequirementsLogbookProfile : Profile
    {
        public SOSSynopticRequirementsLogbookProfile()
        {
            CreateMap<SOSSynopticRequirementsLogbook, SOSSynopticRequirementsLogbookDto>().ReverseMap();
            CreateMap<SOSSynopticRequirementsLogbook, SOSSynopticRequirementsLogbookForCreateDto>().ReverseMap();
            CreateMap<SOSSynopticRequirementsLogbook, SOSSynopticRequirementsLogbookForUpdateDto>().ReverseMap();
        }
    }
}