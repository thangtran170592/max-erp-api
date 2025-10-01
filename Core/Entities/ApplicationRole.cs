using Microsoft.AspNetCore.Identity;

namespace Core.Entities
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public int Level { get; set; } = 1;

        public ApplicationRole() : base() { }

        public ApplicationRole(string roleName) : base(roleName) { }
    }
}