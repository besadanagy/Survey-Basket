namespace Survey_Basket.Contracts.Authentication.Filters
{
    public class PermissionRequirement(string permission): IAuthorizationRequirement
    {
        public string Permission { get; }=permission;
    }
}
