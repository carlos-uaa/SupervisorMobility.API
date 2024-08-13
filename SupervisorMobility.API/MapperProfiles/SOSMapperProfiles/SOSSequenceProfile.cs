using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.Models.SOS.SOSSequenceDtos;

namespace SupervisorMobility.API.MapperProfiles.SOSMapperProfiles
{
    public class SOSSequenceProfile : Profile
    {
        public SOSSequenceProfile()
        {
            CreateMap<SOSSequence, SOSSequenceDto>().ReverseMap();
            CreateMap<SOSSequence, SOSSequenceForCreateDto>().ReverseMap();
            CreateMap<SOSSequence, SOSSequenceForUpdateDto>().ReverseMap();
        }
    }
}