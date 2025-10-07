using Application.Dtos;
using AutoMapper;
using Core.Entities;

namespace Application.Mappings
{
    public class ApprovalFeatureProfile : Profile
    {
        public ApprovalFeatureProfile()
        {
            CreateMap<ApprovalFeatureRequestDto, ApprovalFeature>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.ApprovalConfig, o => o.Ignore())
                .ForMember(d => d.ApprovalSteps, o => o.Ignore())
                .ForMember(d => d.ConcurrencyStamp, o => o.Ignore())
                .ForMember(d => d.UpdatedAt, o => o.Ignore())
                .ForMember(d => d.UpdatedBy, o => o.Ignore())
                .ForMember(d => d.DeletedAt, o => o.Ignore())
                .ForMember(d => d.DeletedBy, o => o.Ignore());

            CreateMap<ApprovalFeature, ApprovalFeatureResponseDto>()
            .ForMember(d => d.ApprovalConfigName, o => o.MapFrom(s => s.ApprovalConfig != null ? s.ApprovalConfig.Name : string.Empty));
        }
    }
}
