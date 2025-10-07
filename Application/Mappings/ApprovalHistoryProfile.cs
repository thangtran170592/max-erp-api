using Application.Dtos;
using AutoMapper;
using Core.Entities;

namespace Application.Mappings;

public class ApprovalHistoryProfile : Profile
{
    public ApprovalHistoryProfile()
    {
        CreateMap<ApprovalHistoryRequestDto, ApprovalHistory>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.StepOrder, o => o.Ignore()) // handled in service
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore());

        CreateMap<ApprovalHistory, ApprovalHistoryResponseDto>();
    }
}
