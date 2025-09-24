namespace Core.Entities
{
    public class BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        private DateTime _updatedAt;
        public DateTime? UpdatedAt
        {
            get
            {
                return _updatedAt;
            }
            set
            {
                _updatedAt = value ?? CreatedAt;
            }
        }
        public virtual string? ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();
    }
}