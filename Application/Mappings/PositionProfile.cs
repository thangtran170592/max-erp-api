using Application.Dtos;
using AutoMapper;
using Core.Entities;

namespace Application.Mappings;

public class PositionProfile : Profile
{
    public PositionProfile()
    {
        CreateMap<PositionRequestDto, Position>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForMember(d => d.Users, o => o.Ignore());
        CreateMap<Position, PositionResponseDto>();
    }
}
