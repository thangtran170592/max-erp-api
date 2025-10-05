namespace Core.Entities
{
    public class Position : BaseEntity
    {
        public string Uid { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool Status { get; set; } = true;
        public virtual ICollection<ApplicationUser> Users { get; set; } = [];
    }
}