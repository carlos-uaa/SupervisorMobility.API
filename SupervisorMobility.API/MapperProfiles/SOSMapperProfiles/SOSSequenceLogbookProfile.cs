using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.Models.SOS.SOSSequenceDtos;
using SupervisorMobility.API.Models.SOS.SOSSequenceLogbookDtos;

namespace SupervisorMobility.API.MapperProfiles.SOSMapperProfiles
{
    public class SOSSequenceLogbookProfile : Profile
    {
        public SOSSequenceLogbookProfile()
        {
            CreateMap<SOSSequenceLogbook, SOSSequenceLogbookDto>().ReverseMap();
            CreateMap<SOSSequenceLogbook, SOSSequenceLogbookForCreateDto>().ReverseMap();
            CreateMap<SOSSequenceLogbook, SOSSequenceLogbookForUpdateDto>().ReverseMap();
        }
    }
}