using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.Models.SOS.SOSDistributionDtos;
using SupervisorMobility.API.Models.SOS.SOSDistributionOperationSequenceDtos;

namespace SupervisorMobility.API.MapperProfiles.SOSMapperProfiles
{
    public class SOSDistributionOperationSequenceProfile : Profile
    {
        public SOSDistributionOperationSequenceProfile()
        {
            CreateMap<SOSDistributionOperationSequence, SOSDistributionOperationSequenceDto>().ReverseMap();
            CreateMap<SOSDistributionOperationSequence, SOSDistributionOperationSequenceForCreateDto>().ReverseMap();
            CreateMap<SOSDistributionOperationSequence, SOSDistributionOperationSequenceForUpdateDto>().ReverseMap();
        }
    }
}