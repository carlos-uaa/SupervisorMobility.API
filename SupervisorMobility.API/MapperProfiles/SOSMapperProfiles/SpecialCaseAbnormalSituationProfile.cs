using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.Models.SOS.SOSAnalysisDtos;
using SupervisorMobility.API.Models.SOS.SpecialCaseAbnormalSituationDtos;

namespace SupervisorMobility.API.MapperProfiles.SOSMapperProfiles
{
    public class SpecialCaseAbnormalSituationProfile : Profile
    {
        public SpecialCaseAbnormalSituationProfile()
        {
            CreateMap<SpecialCaseAbnormalSituation, SpecialCaseAbnormalSituationDto>().ReverseMap();
            CreateMap<SpecialCaseAbnormalSituation, SpecialCaseAbnormalSituationForCreateDto>().ReverseMap();
            CreateMap<SpecialCaseAbnormalSituation, SpecialCaseAbnormalSituationForUpdateDto>().ReverseMap();
        }
    }
}