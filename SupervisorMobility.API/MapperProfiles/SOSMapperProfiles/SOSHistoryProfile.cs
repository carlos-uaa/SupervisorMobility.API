using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.DataAccess.Entities.SOS.History;
using SupervisorMobility.API.Models.SOS.EquipmentDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos.AnalysisBkupDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos.AnalysisDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos.SectionDtos;
using SupervisorMobility.API.Models.SOS.SOSHubDtos;

namespace SupervisorMobility.API.MapperProfiles.SOSMapperProfiles
{
    public class SOSHistoryProfile : Profile
    {
        public SOSHistoryProfile() {
            CreateMap<SOSHub, SOSHubHistory>().ReverseMap();
            CreateMap<SOSHub, SOSHubHistoryForCreateDto>().ReverseMap();

            CreateMap<SOSHubHistory, SOSHubHistoryDto>().ReverseMap();
            CreateMap<SOSHubHistory, SOSHubHistoryForCreateDto>().ReverseMap();

            CreateMap<SOSHubHistory, SOSHubDto>().ReverseMap();
            CreateMap<SOSHubHistory, SOSHubForCreateDto>().ReverseMap();
            CreateMap<SOSHubHistory, SOSHubForUpdateDto>().ReverseMap();


            CreateMap<Section, SectionHistory>().ReverseMap();
            CreateMap<SectionDto, SectionHistory>().ReverseMap();

            CreateMap<Analysis, AnalysisHistory>().ReverseMap();
            CreateMap<AnalysisDto, AnalysisHistory>().ReverseMap();

            CreateMap<AnalysisBkup, AnalysisBkupHistory>().ReverseMap();
            CreateMap<AnalysisBkupDto, AnalysisBkupHistory>().ReverseMap();


        }
    }
}
