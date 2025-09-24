namespace Application.Common.Security
{
    public static class Role
    {
        public const string Admin = "Admin";
        public const string Manager = "Manager";
        public const string Customer = "Customer";
        public const string Guest = "Guest";

        public static readonly string[] AllRoles = [Admin, Manager, Customer, Guest];
    }
}
