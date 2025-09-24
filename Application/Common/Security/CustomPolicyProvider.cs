using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Application.Common.Security
{
    public class CustomPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        public CustomPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
        {
        }

        public override Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith("PERMISSION:"))
            {
                var permission = policyName["PERMISSION:".Length..];

                var policy = new AuthorizationPolicyBuilder()
                    .AddRequirements(new PermissionRequirement(permission))
                    .Build();

                return Task.FromResult<AuthorizationPolicy?>(policy);
            }

            return base.GetPolicyAsync(policyName);
        }
    }
}