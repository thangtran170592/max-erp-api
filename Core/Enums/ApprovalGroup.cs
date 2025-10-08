namespace Core.Enums
{
    public enum ApprovalGroup
    {
        Warehouse = 1,
        Product = 2,
        Supplier = 3,
        Inbound = 4,
        Outbound = 5,
    }

    public static class ApprovalGroupExtensions
    {
        public static string GetTitle(this ApprovalGroup group) => group switch
        {
            ApprovalGroup.Warehouse => "Warehouse",
            ApprovalGroup.Product => "Product",
            ApprovalGroup.Supplier => "Supplier",
            ApprovalGroup.Inbound => "Inbound",
            ApprovalGroup.Outbound => "Outbound",
            _ => throw new NotImplementedException()
        };
    }
}