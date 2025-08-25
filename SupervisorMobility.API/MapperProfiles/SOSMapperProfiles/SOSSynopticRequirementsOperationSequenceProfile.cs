using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.Models.SOS.SOSSynopticTableofOperatingRequirementsOperationSequenceDtos;

namespace SupervisorMobility.API.MapperProfiles.SOSMapperProfiles
{
    public class SOSSynopticRequirementsOperationSequenceProfile : Profile
    {
        public SOSSynopticRequirementsOperationSequenceProfile()
        {
            CreateMap<SOSSynopticRequirementsOperationSequence, SOSSynopticRequirementsOperationSequenceDto>().ReverseMap();
            CreateMap<SOSSynopticRequirementsOperationSequence, SOSSynopticRequirementsOperationSequenceForCreateDto>().ReverseMap();
            CreateMap<SOSSynopticRequirementsOperationSequence, SOSSynopticRequirementsOperationSequenceForUpdateDto>().ReverseMap();
        }
    }
}