using Application.Dtos;
using AutoMapper;
using Core.Entities;

namespace Application.Mappings
{
    public class ChatProfile : Profile
    {
        public ChatProfile()
        {
            CreateMap<Message, MessageResponseDto>();
            CreateMap<Room, RoomResponseDto>();
            CreateMap<RoomRequestDto, Room>();
            CreateMap<BroadcastRequestDto, Broadcast>();
        }
    }
}