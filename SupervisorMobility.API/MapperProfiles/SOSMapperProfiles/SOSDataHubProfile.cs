using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Entities.SOS.History;
using SupervisorMobility.API.Models.SOS.SOSHubDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos.AnalysisBkupDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos.AnalysisDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos.SectionDtos;

namespace SupervisorMobility.API.MapperProfiles.SOSMapperProfiles
{
    public class SOSDataHubProfile : Profile
    {
        public SOSDataHubProfile()
        {
            CreateMap<SOSHub, SOSHubDto>().ReverseMap();
            CreateMap<SOSHub, SOSHubForCreateDto>().ReverseMap();
            CreateMap<SOSHubForUpdateDto, SOSHubForCreateDto>().ReverseMap();
            CreateMap<SOSHub, SOSHubForUpdateDto>().ReverseMap();

            CreateMap<Section, SectionDto>().ReverseMap();
            CreateMap<Section, SectionForUpdateDto>().ReverseMap();
            CreateMap<Section, SectionForCreateDto>().ReverseMap();

            CreateMap<Analysis, AnalysisDto>().ReverseMap();
            CreateMap<Analysis, AnalysisForCreateDto>().ReverseMap();
            CreateMap<Analysis, AnalysisForUpdateDto>().ReverseMap();

            CreateMap<AnalysisBkup, AnalysisBkupDto>().ReverseMap();
            CreateMap<AnalysisBkup, AnalysisBkupForCreateDto>().ReverseMap();
            CreateMap<AnalysisBkup, AnalysisBkupForUpdateDto>().ReverseMap();
           
        }
    }
}