namespace Core.Entities
{
    public class UnitOfMeasure : BaseEntity
    {
        public required string Uid { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool Status { get; set; }
    }
}