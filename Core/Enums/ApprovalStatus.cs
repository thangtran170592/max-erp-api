namespace Core.Enums
{
    public enum ApprovalStatus
    {
        Approved = 1,
        Pending = 2,
        Rejected = 3
    }
    
    public static class ApprovalStatusExtensions
    {
        public static string GetTitle(this ApprovalStatus status) => status switch
        {
            ApprovalStatus.Approved => "Approved",
            ApprovalStatus.Pending => "Pending",
            ApprovalStatus.Rejected => "Rejected",
            _ => "Unknown"
        };
    }
}