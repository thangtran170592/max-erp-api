using Application.Dtos;
using AutoMapper;
using Core.Entities;

namespace Application.Mappings
{
    public class ChatProfile : Profile
    {
        public ChatProfile()
        {
            CreateMap<Message, MessageResponseDto>()
                        .ForMember(d => d.SenderName, opt => opt.MapFrom(s => s.Sender.UserName));
            CreateMap<MessageReceipt, MessageReceiptDto>()
                .ForMember(d => d.UserName, opt => opt.MapFrom(s => s.User.UserName));
            CreateMap<ConversationMember, ConversationMemberDto>()
                .ForMember(d => d.UserName, opt => opt.MapFrom(s => s.User.UserName));
            CreateMap<Broadcast, BroadcastResponseDto>()
                .ForMember(d => d.SenderName, opt => opt.MapFrom(s => s.Sender.UserName));
            CreateMap<BroadcastRecipient, BroadcastRecipientDto>()
                .ForMember(d => d.UserName, opt => opt.MapFrom(s => s.User.UserName));
                CreateMap<Conversation, ConversationResponseDto>()
            .ForMember(
                dest => dest.IsVirtual,
                opt => opt.MapFrom(src => src.Id == Guid.Empty)
            );
        }
    }
}