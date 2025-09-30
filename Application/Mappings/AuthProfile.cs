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
    }
}