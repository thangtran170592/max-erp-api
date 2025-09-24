namespace Application.Common.Security
{
    public static class RolePermission
    {
        public static readonly Dictionary<string, string[]> RolePermissions = new()
        {
            [Role.Admin] = Permission.AllPermissions,

            [Role.Manager] = [
                Permission.ViewUsers,
                Permission.CreateUsers,
                Permission.UpdateUsers,
                Permission.ViewSettings,
                Permission.CreateChats,
                Permission.ViewChats,
                Permission.ViewRoles,
            ],

            [Role.Customer] = [
                Permission.ViewRoles,
                Permission.ViewUsers,
                Permission.ViewChats,
                Permission.CreateChats,
            ],

            [Role.Guest] = [
                Permission.ViewUsers
            ]
        };

        /// <summary>
        /// Get all permissions for a specific role
        /// </summary>
        /// <param name="roleName">Role name</param>
        /// <returns>Array of permissions for the role</returns>
        public static string[] GetPermissionsForRole(string roleName)
        {
            return RolePermissions.TryGetValue(roleName, out var permissions)
                ? permissions
                : [];
        }

        /// <summary>
        /// Check if a role has a specific permission
        /// </summary>
        /// <param name="roleName">Role name</param>
        /// <param name="permission">Permission to check</param>
        /// <returns>True if role has permission</returns>
        public static bool RoleHasPermission(string roleName, string permission)
        {
            var permissions = GetPermissionsForRole(roleName);
            return permissions.Contains(permission);
        }

        /// <summary>
        /// Get all roles that have a specific permission
        /// </summary>
        /// <param name="permission">Permission to check</param>
        /// <returns>Array of roles that have the permission</returns>
        public static string[] GetRolesWithPermission(string permission)
        {
            return [.. RolePermissions
                .Where(kvp => kvp.Value.Contains(permission))
                .Select(kvp => kvp.Key)];
        }
    }
}
