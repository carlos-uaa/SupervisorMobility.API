using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.Models.SOS.SOSCombinationDtos;
using SupervisorMobility.API.Models.SOS.SOSCombinationOperationSequenceDtos;

namespace SupervisorMobility.API.MapperProfiles.SOSMapperProfiles
{
    public class SOSCombinationOperationSequenceProfile : Profile
    {
        public SOSCombinationOperationSequenceProfile()
        {
            CreateMap<SOSCombinationOperationSequence, SOSCombinationOperationSequenceDto>().ReverseMap();
            CreateMap<SOSCombinationOperationSequence, SOSCombinationOperationSequenceForCreateDto>().ReverseMap();
            CreateMap<SOSCombinationOperationSequence, SOSCombinationOperationSequenceForUpdateDto>().ReverseMap();
        }
    }
}