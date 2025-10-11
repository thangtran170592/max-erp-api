using Application.Dtos;
using AutoMapper;
using Core.Entities;

namespace Application.Mappings;

public class ApprovalDocumentProfile : Profile
{
    public ApprovalDocumentProfile()
    {
        CreateMap<ApprovalDocumentDto, ApprovalDocument>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForMember(d => d.ApprovalHistories, o => o.Ignore());

        CreateMap<ApprovalDocument, ApprovalDocumentResponseDto>();
    }
}
