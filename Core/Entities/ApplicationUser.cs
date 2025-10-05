using Core.Enums;
using Microsoft.AspNetCore.Identity;

namespace Core.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public int OrderNumber { get; private set; }
        public string Uid { get; set; } = string.Empty;
        public Guid? DepartmentId { get; set; }
        public Guid? PositionId { get; set; }
        public virtual Department? Department { get; set; }
        public virtual Position? Position { get; set; }
        public string? Description { get; set; }
        public string? FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? ProfilePicture { get; set; }
        public AccountStatus AccountStatus { get; set; } = AccountStatus.Active;
        public bool IsActive { get; set; } = true;
        public DateTime? JoinDate { get; set; }
        public Gender Gender { get; set; } = Gender.Unknown;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
        public string? Address { get; set; }
        public Level Level { get; set; } = Level.Beginner;
        public bool IsOnline { get; set; } = false;
        public string? LastSeenAt { get; set; }
        public UserStatus Status { get; set; } = UserStatus.Available;
        public string? StatusMessage { get; set; }
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = [];

        // Chat related collections
        public virtual ICollection<Message> SentMessages { get; set; } = [];
        public virtual ICollection<MessageReceipt> MessageReceipts { get; set; } = [];
        public virtual ICollection<ConversationMember> Conversations { get; set; } = [];
        public virtual ICollection<Broadcast> SentBroadcasts { get; set; } = [];
        public virtual ICollection<BroadcastRecipient> ReceivedBroadcasts { get; set; } = [];

        public void AddRefreshToken(RefreshToken refreshToken)
        {
            RefreshTokens.Add(refreshToken);
        }

        public void UpdateLastSeen()
        {
            LastSeenAt = DateTime.UtcNow.ToString("O");
        }

        public void UpdateStatus(UserStatus status, string? message = null)
        {
            Status = status;
            StatusMessage = message;
        }
    }
}