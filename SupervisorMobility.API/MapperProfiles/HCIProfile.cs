using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities;
using SupervisorMobility.API.DataAccess.Entities.SOS;

namespace SupervisorMobility.API.MapperProfiles
{
    public class HCIProfile : Profile
    {
        public HCIProfile()
        {
            CreateMap<HCI, Models.HCIDtos.HCIDto>()
                .ForMember(dest => dest.ILUs, opt => opt.MapFrom(src => src.User != null ? src.User.ILURegisers : null))
                .ReverseMap();

            CreateMap<HCI, Models.HCIDtos.CreateHCIDto>().ReverseMap();
            CreateMap<HCI, Models.HCIDtos.UpdateHCIDto>().ReverseMap();
            CreateMap<HCITransaction, Models.HCITransactionDtos.HCITransactionDto>().ReverseMap();
            CreateMap<HCITransaction, Models.HCITransactionDtos.CreateHCITransactionDto>().ReverseMap();
            CreateMap<HCITransaction, Models.HCITransactionDtos.UpdateHCITransactionDto>().ReverseMap();
            CreateMap<HCICategory, Models.HCICategoryDtos.HCICategoryDto>().ReverseMap();
            CreateMap<HCICategory, Models.HCICategoryDtos.CreateHCICategoryDto>().ReverseMap();
            CreateMap<HCICategory, Models.HCICategoryDtos.UpdateHCICategoryDto>().ReverseMap();

            CreateMap<Commentary, Models.CommentaryDtos.CommentaryDto>().ReverseMap();
            CreateMap<Commentary, CommentaryHistory>().ReverseMap();
            CreateMap<CommentaryHistory, Models.CommentaryDtos.CommentaryDto>().ReverseMap();
            CreateMap<Commentary, Models.CommentaryDtos.CreateCommentaryDto>().ReverseMap();
            CreateMap<Commentary, Models.CommentaryDtos.UpdateCommentaryDto>().ReverseMap();
        }
    }
}
