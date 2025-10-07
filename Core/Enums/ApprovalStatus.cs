namespace Core.Enums
{
    public enum ApprovalStatus
    {
        Draft = 1,
        Pending = 2,
        Approved = 3,
        Rejected = 4
    }

    public static class ApprovalStatusExtensions
    {
        public static string GetTitle(this ApprovalStatus status) => status switch
        {
            ApprovalStatus.Draft => "Draft",
            ApprovalStatus.Approved => "Approved",
            ApprovalStatus.Pending => "Pending",
            ApprovalStatus.Rejected => "Rejected",
            _ => throw new NotImplementedException()
        };
    }   
}