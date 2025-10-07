using Application.Dtos;
using AutoMapper;
using Core.Entities;

namespace Application.Mappings;

public class ApprovalRequestProfile : Profile
{
    public ApprovalRequestProfile()
    {
        CreateMap<ApprovalRequestDto, ApprovalRequest>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForMember(d => d.ApprovalHistories, o => o.Ignore());

        CreateMap<ApprovalRequest, ApprovalResponseDto>();
    }
}
