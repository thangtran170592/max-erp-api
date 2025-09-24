using Application.Common.Models;
using Application.Dtos;
using AutoMapper;
using Core.Entities;

namespace Application.Mappings;

public class AuthProfile : Profile
{
    public AuthProfile()
    {
        CreateMap<User, UserResponseDto>();
        CreateMap(typeof(PagedResult<>), typeof(MetaData))
            .ForMember("TotalCount", opt => opt.MapFrom("TotalCount"))
            .ForMember("TotalPages", opt => opt.MapFrom("TotalPages"))
            .ForMember("Page", opt => opt.MapFrom("Page"))
            .ForMember("PageSize", opt => opt.MapFrom("PageSize"));
        CreateMap(typeof(PagedResult<>), typeof(PagedResult<>));
    }
}