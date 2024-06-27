using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.Models.SOS.SOSAnalysisDtos;
using SupervisorMobility.API.Models.SOS.SOSAnalysisLogbookDtos;

namespace SupervisorMobility.API.MapperProfiles.ISOSMapperProfiles
{
    public class SOSAnalysisLogbookProfile : Profile
    {
        public SOSAnalysisLogbookProfile()
        {
            CreateMap<SOSAnalysisLogbook, SOSAnalysisLogbookDto>().ReverseMap();
            CreateMap<SOSAnalysisLogbook, SOSAnalysisLogbookForCreateDto>().ReverseMap();
            CreateMap<SOSAnalysisLogbook, SOSAnalysisLogbookForUpdateDto>().ReverseMap();
        }
    }
}