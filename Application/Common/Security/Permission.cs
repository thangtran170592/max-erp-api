namespace Application.Common.Security
{
    public static class Permission
    {
        // User Management
        public const string ViewUsers = "users.view";
        public const string CreateUsers = "users.create";
        public const string ImportUsers = "users.import";
        public const string UpdateUsers = "users.update";
        public const string DeleteUsers = "users.delete";

        // Chat Management
        public const string ViewChats = "chats.view";
        public const string CreateChats = "chats.create";
        public const string UpdateChats = "chats.update";
        public const string DeleteChats = "chats.delete";

        // System Management
        public const string ViewSettings = "settings.view";
        public const string UpdateSettings = "settings.update";

        // Role Management
        public const string ViewRoles = "roles.view";
        public const string CreateRoles = "roles.create";
        public const string UpdateRoles = "roles.update";
        public const string DeleteRoles = "roles.delete";
        public const string AssignRoles = "roles.assign";

        // All permissions
        public static readonly string[] AllPermissions = [
            ViewUsers, CreateUsers, UpdateUsers, DeleteUsers, ImportUsers,
            ViewChats, CreateChats, UpdateChats, DeleteChats,
            ViewSettings, UpdateSettings,
            ViewRoles, CreateRoles, UpdateRoles, DeleteRoles, AssignRoles
        ];
    }
}
