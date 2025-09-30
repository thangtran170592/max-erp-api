namespace Core.Enums
{
    public enum ApprovalStatus
    {
        Pending = 1,
        Approved = 2,
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