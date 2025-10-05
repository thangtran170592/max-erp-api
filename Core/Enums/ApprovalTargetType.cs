namespace Core.Enums
{
    public enum ApprovalTargetType
    {
        User = 1,
        Position = 2,
        Department = 3
    }

    public static class ApprovalTargetTypeExtensions
    {
        public static string GetTitle(this ApprovalTargetType targetType) => targetType switch
        {
            ApprovalTargetType.User => "User",
            ApprovalTargetType.Position => "Position",
            ApprovalTargetType.Department => "Department",
            _ => throw new NotImplementedException()
        };
    }
}