using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Enums;

namespace Core.Entities
{
    public class ApprovalFeature : BaseEntity
    {
        public string Uid { get; set; } = string.Empty;
        public Guid ApprovalConfigId { get; set; }
        public ApprovalConfig ApprovalConfig { get; set; } = null!;
        public bool Status { get; set; } = true;
        public ApprovalTargetType TargetType { get; set; }
        public string TargetValue { get; set; } = string.Empty;
        public ICollection<ApprovalStep> ApprovalStep { get; set; } = [];

    }
}