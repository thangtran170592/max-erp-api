namespace Core.Entities
{
    public class BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }
        private DateTime? _updatedAt;
        public DateTime? UpdatedAt
        {
            get => _updatedAt;
            set
            {
                if (CreatedAt != DateTime.UtcNow)
                {
                    _updatedAt = DateTime.UtcNow;
                }
                else    
                {
                    _updatedAt = null;
                }
            }
        }
        public Guid? UpdatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public Guid? SubmittedBy { get; set; }
        public virtual Guid? ConcurrencyStamp { get; set; } = Guid.NewGuid();
    }
}