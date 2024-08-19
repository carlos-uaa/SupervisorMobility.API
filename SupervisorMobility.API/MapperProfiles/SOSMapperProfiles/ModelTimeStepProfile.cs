using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities.SOS;
using SupervisorMobility.API.Models.SOS.ModelTimeStepDtos;

namespace SupervisorMobility.API.MapperProfiles.SOSMapperProfiles
{
    public class ModelTimeStepProfile : Profile
    {
        public ModelTimeStepProfile()
        {
            CreateMap<ModelTimeStep, ModelTimeStepDto>().ReverseMap();
            CreateMap<ModelTimeStep, ModelTimeStepForCreateDto>().ReverseMap();
            CreateMap<ModelTimeStep, ModelTimeStepForUpdateDto>().ReverseMap();
        }
    }
}