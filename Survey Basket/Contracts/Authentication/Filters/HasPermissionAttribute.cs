namespace Survey_Basket.Contracts.Authentication.Filters
{
    public class HasPermissionAttribute(string permission):AuthorizeAttribute(permission)
    {

    }
}
