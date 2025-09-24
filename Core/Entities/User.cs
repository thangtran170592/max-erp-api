using Core.Enums;
using Microsoft.AspNetCore.Identity;

namespace Core.Entities
{
    public class User : IdentityUser<Guid>
    {
        public int OrderNumber { get; private set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string DepartmentCode { get; set; } = string.Empty;
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

        public ICollection<Message> SentMessages { get; set; } = [];
        public ICollection<Message> ReceivedMessages { get; set; } = [];
        public ICollection<RoomUser> Rooms { get; set; } = new List<RoomUser>();
        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = [];

        public void AddRefreshToken(RefreshToken refreshToken)
        {
            RefreshTokens.Add(refreshToken);
        }
    }
}