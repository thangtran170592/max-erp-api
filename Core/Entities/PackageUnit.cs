using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class PackageUnit : BaseEntity
    {
        public required Guid PackageId { get; set; }
        public virtual Package? Package { get; set; }
        public int Level { get; set; }
        public decimal Quantity { get; set; }
        public Guid UnitId { get; set; }
        public virtual UnitOfMeasure? Unit { get; set; }
    }
}