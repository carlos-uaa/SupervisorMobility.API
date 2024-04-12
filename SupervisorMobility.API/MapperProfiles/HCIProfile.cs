using AutoMapper;
using SupervisorMobility.API.DataAccess.Entities;

namespace SupervisorMobility.API.MapperProfiles
{
    public class HCIProfile : Profile
    {
        public HCIProfile()
        {
            CreateMap<HCI, Models.HCIDtos.HCIDto>().ReverseMap();
            CreateMap<HCI, Models.HCIDtos.CreateHCIDto>().ReverseMap();
            CreateMap<HCI, Models.HCIDtos.UpdateHCIDto>().ReverseMap();
            CreateMap<HCITransaction, Models.HCITransactionDtos.HCITransactionDto>().ReverseMap();
            CreateMap<HCITransaction, Models.HCITransactionDtos.CreateHCITransactionDto>().ReverseMap();
            CreateMap<HCITransaction, Models.HCITransactionDtos.UpdateHCITransactionDto>().ReverseMap();
            CreateMap<HCICategory, Models.HCICategoryDtos.HCICategoryDto>().ReverseMap();
            CreateMap<HCICategory, Models.HCICategoryDtos.CreateHCICategoryDto>().ReverseMap();
            CreateMap<HCICategory, Models.HCICategoryDtos.UpdateHCICategoryDto>().ReverseMap();
            CreateMap<Commentary, Models.CommentaryDtos.CommentaryDto>().ReverseMap();
            CreateMap<Commentary, Models.CommentaryDtos.CreateCommentaryDto>().ReverseMap();
            CreateMap<Commentary, Models.CommentaryDtos.UpdateCommentaryDto>().ReverseMap();
        }
    }
}
