using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.Models.SOS.SOSAnalysisDtos;

namespace SupervisorMobility.API.MapperProfiles.SOSMapperProfiles
{
    public class SOSAnalysisProfile : Profile
    {
        public SOSAnalysisProfile()
        {
            CreateMap<SOSAnalysis, SOSAnalysisDto>().ReverseMap();
            CreateMap<SOSAnalysis, SOSAnalysisForCreateDto>().ReverseMap();
            CreateMap<SOSAnalysis, SOSAnalysisForUpdateDto>().ReverseMap().ForMember(dest => dest.SOSHub, opt => opt.Ignore());
        }
    }
}