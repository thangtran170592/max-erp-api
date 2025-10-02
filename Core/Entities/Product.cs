using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Product : BaseEntity
    {
        public required string Uid { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public Guid CategoryId { get; set; }
        public virtual ProductCategory? Category { get; set; }
        public Guid PackageId { get; set; }
        public virtual Package? Package { get; set; }
        public Guid UnitOfMeasureId { get; set; }
        public virtual UnitOfMeasure? UnitOfMeasure { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public bool Status { get; set; }
    }
}