
namespace Survey_Basket.Contracts.Authentication.Filters
{
    public class PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) 
        : DefaultAuthorizationPolicyProvider(options)
    {
        private readonly AuthorizationOptions authorizationOptions=options.Value;
        public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            var policy= await base.GetPolicyAsync(policyName);
            if(policy is not null)
            return policy;

            var PermissionPolicy =new AuthorizationPolicyBuilder()
                .AddRequirements(new PermissionRequirement(policyName))
                .Build();
            authorizationOptions.AddPolicy(policyName, PermissionPolicy);
            return PermissionPolicy;
        }
    }
}
