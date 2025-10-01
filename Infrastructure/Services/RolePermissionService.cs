using Application.Common.Security;
using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class RolePermissionService : IRolePermissionService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public RolePermissionService(
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task InitializeRolesAndPermissionsAsync()
        {
            // Create roles if they don't exist
            foreach (var roleName in Role.AllRoles)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new ApplicationRole(roleName));
                }
            }

            // Add permissions as claims to roles
            foreach (var rolePermission in RolePermission.RolePermissions)
            {
                var role = await _roleManager.FindByNameAsync(rolePermission.Key);
                if (role != null)
                {
                    // Remove existing permission claims
                    var existingClaims = await _roleManager.GetClaimsAsync(role);
                    var permissionClaims = existingClaims.Where(c => c.Type == "Permission");
                    foreach (var claim in permissionClaims)
                    {
                        await _roleManager.RemoveClaimAsync(role, claim);
                    }

                    // Add new permission claims
                    foreach (var permission in rolePermission.Value)
                    {
                        var claim = new System.Security.Claims.Claim("Permission", permission);
                        await _roleManager.AddClaimAsync(role, claim);
                    }
                }
            }
        }

        public async Task<string[]> GetUserPermissionsAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Array.Empty<string>();

            var userRoles = await _userManager.GetRolesAsync(user);
            var permissions = new HashSet<string>();

            foreach (var roleName in userRoles)
            {
                var rolePermissions = RolePermission.GetPermissionsForRole(roleName);
                foreach (var permission in rolePermissions)
                {
                    permissions.Add(permission);
                }
            }

            return permissions.ToArray();
        }

        public async Task<bool> UserHasPermissionAsync(string userId, string permission)
        {
            var userPermissions = await GetUserPermissionsAsync(userId);
            return userPermissions.Contains(permission);
        }

        public string[] GetRolePermissions(string roleName)
        {
            return RolePermission.GetPermissionsForRole(roleName);
        }

        public bool RoleHasPermission(string roleName, string permission)
        {
            return RolePermission.RoleHasPermission(roleName, permission);
        }
    }
}
