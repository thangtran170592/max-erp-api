using Application.Dtos;
using AutoMapper;
using Core.Entities;

namespace Application.Mappings
{
    public class ApprovalConfigProfile : Profile
    {
        public ApprovalConfigProfile()
        {
            CreateMap<ApprovalConfigRequestDto, ApprovalConfig>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.ApprovalFeatures, o => o.Ignore())
                .ForMember(d => d.ConcurrencyStamp, o => o.Ignore())
                .ForMember(d => d.UpdatedAt, o => o.Ignore())
                .ForMember(d => d.UpdatedBy, o => o.Ignore())
                .ForMember(d => d.DeletedAt, o => o.Ignore())
                .ForMember(d => d.DeletedBy, o => o.Ignore());

            CreateMap<ApprovalConfig, ApprovalConfigResponseDto>();
        }
    }
}
