using Application.Dtos;
using AutoMapper;
using Core.Entities;

namespace Application.Mappings
{
    public class ApprovalStepProfile : Profile
    {
        public ApprovalStepProfile()
        {
            CreateMap<ApprovalStepRequestDto, ApprovalStep>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.ApprovalFeature, o => o.Ignore())
                .ForMember(d => d.ConcurrencyStamp, o => o.Ignore())
                .ForMember(d => d.UpdatedAt, o => o.Ignore())
                .ForMember(d => d.UpdatedBy, o => o.Ignore())
                .ForMember(d => d.DeletedAt, o => o.Ignore())
                .ForMember(d => d.DeletedBy, o => o.Ignore())
                .ForMember(d => d.StepOrder, o => o.Ignore()); // We compute if not provided

            CreateMap<ApprovalStep, ApprovalStepResponseDto>();
        }
    }
}
