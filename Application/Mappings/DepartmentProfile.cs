using Application.Dtos;
using AutoMapper;
using Core.Entities;

namespace Application.Mappings;

public class DepartmentProfile : Profile
{
    public DepartmentProfile()
    {
        CreateMap<DepartmentRequestDto, Department>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForMember(d => d.Users, o => o.Ignore());
        CreateMap<Department, DepartmentResponseDto>();
    }
}
