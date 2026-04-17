using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities.HRIRevisionsItem_Entities;
using SupervisorMobility.API.Models.HRICyclesDtos;
using SupervisorMobility.API.Models.HRIDailyRevisionDtos;
using SupervisorMobility.API.Models.HRIDtos;
using SupervisorMobility.API.Models.HRIHourmeterRevisionDto;
using SupervisorMobility.API.Models.HRIRevisionCycles;
using SupervisorMobility.API.Models.HRIRevisionItemsDtos;

namespace SupervisorMobility.API.MapperProfiles
{
    public class HRIProfiles : Profile
    {
        public HRIProfiles()
        {
               CreateMap<HRI,GetHRIDto>().ReverseMap();
                CreateMap<CreateHRIDto,HRI>().ReverseMap();

            #region HRICycles
                CreateMap<HRICycles, GetHRICyclesDto>().ReverseMap();
                CreateMap<CreateHRICyclesDto, HRICycles>().ReverseMap();
                CreateMap<UpdateHRICycleDto, HRICycles>().ReverseMap();
            #endregion

            #region HRIRevisionItems
            CreateMap<HRIRevisionItems, GetHRIRevisionItemDto>().ReverseMap();
            CreateMap<CreateHRIRevisionItemDto, HRIRevisionItems>().ReverseMap();
            CreateMap<UpdateHRIRevisionItemDto, HRIRevisionItems>().ReverseMap();
            CreateMap<Frequency, GetFrequencyDto>().ReverseMap();
            CreateMap<CreateFrequencyDto, Frequency>().ReverseMap();
            CreateMap<UpdateFrequencyDto, Frequency>().ReverseMap();
            CreateMap<Veredict, GetVeredictDto>().ReverseMap();
            CreateMap<CreateVeredictDto, Veredict>().ReverseMap();
            CreateMap<UpdateVeredictDto, Veredict>().ReverseMap();
            CreateMap<RevisionMethod, GetRevisionMethodDto>().ReverseMap();
            CreateMap<CreateRevisionMethodDto, RevisionMethod>().ReverseMap();
            CreateMap<UpdateRevisionMethodDto, RevisionMethod>().ReverseMap();


            #endregion

            #region RevisionCycles  
            CreateMap<RevisionCycles, GetRevisionCyclesDto>().ReverseMap();
            CreateMap<CreateRevisionCyclesDto, RevisionCycles>().ReverseMap();
            CreateMap<UpdateRevisionCycleDto, RevisionCycles>().ReverseMap();
            #endregion

            #region dailyRevisions
            CreateMap<CreateDailyRevisionDto,DailyRevisions>().ReverseMap();
            CreateMap<DailyRevisions, GetDailyRevisionDto>().ReverseMap();
           
            #endregion
            #region HourmeterRevision
            CreateMap<HourmeterRevision, GetHourmeterRevisionDto>().ReverseMap();
            CreateMap<CreateHourMeterRevisionDto, HourmeterRevision>().ReverseMap();
            #endregion
        }
    }
}
