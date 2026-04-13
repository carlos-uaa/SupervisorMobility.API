using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities.HRI;
using SupervisorMobility.API.DataAccess.Entities.HRI_s_Entities.HRIRevisionsItem_Entities;
using SupervisorMobility.API.Models.HRICyclesDtos;
using SupervisorMobility.API.Models.HRIDtos;
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
            #endregion

            #region HRIRevisionItems
            //CreateMap<HRIRevisionItems, GetHRIRevisionItemsDto>().ReverseMap();
            //CreateMap<CreateHRIRevisionItemsDto, HRIRevisionItems>().ReverseMap();
            //CreateMap<UpdateHRIRevisionItemsDto, HRIRevisionItems>().ReverseMap();
            CreateMap<Frequency, GetFrequencyDto>().ReverseMap();
            CreateMap<CreateFrequencyDto, Frequency>().ReverseMap();
            CreateMap<Veredict, GetVeredictDto>().ReverseMap();
            CreateMap<CreateVeredictDto, Veredict>().ReverseMap();
            CreateMap<RevisionMethod, GetRevisionMethodDto>().ReverseMap();
            CreateMap<CreateRevisionMethodDto, RevisionMethod>().ReverseMap();


            #endregion
        }
    }
}
