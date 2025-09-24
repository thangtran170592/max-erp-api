using Core.Enums;

namespace Core.Entities
{
    public class Broadcast : BaseEntity
    {
        public Guid SenderId { get; set; }
        public string Content { get; set; } = null!;
        public MessageType Type { get; set; } = MessageType.Text;
        public string TargetType { get; set; } = "All"; // All, Department, Group
        public Guid? TargetId { get; set; } // DepartmentId / RoomId / null if All
        public virtual User Sender { get; set; } = null!;
    }
}