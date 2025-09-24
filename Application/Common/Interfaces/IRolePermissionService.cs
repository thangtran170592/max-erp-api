namespace Application.Common.Security
{
    public interface IRolePermissionService
    {
        /// <summary>
        /// Initialize default roles and permissions in the system
        /// </summary>
        Task InitializeRolesAndPermissionsAsync();

        /// <summary>
        /// Get all permissions for a user based on their roles
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Array of permissions</returns>
        Task<string[]> GetUserPermissionsAsync(string userId);

        /// <summary>
        /// Check if user has a specific permission
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="permission">Permission to check</param>
        /// <returns>True if user has permission</returns>
        Task<bool> UserHasPermissionAsync(string userId, string permission);

        /// <summary>
        /// Get all permissions for a role
        /// </summary>
        /// <param name="roleName">Role name</param>
        /// <returns>Array of permissions</returns>
        string[] GetRolePermissions(string roleName);

        /// <summary>
        /// Check if role has a specific permission
        /// </summary>
        /// <param name="roleName">Role name</param>
        /// <param name="permission">Permission to check</param>
        /// <returns>True if role has permission</returns>
        bool RoleHasPermission(string roleName, string permission);
    }
}
