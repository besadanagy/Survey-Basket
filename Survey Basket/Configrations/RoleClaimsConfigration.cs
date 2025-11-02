
namespace Survey_Basket.validations
{
    public class RoleClaimsConfigration : IEntityTypeConfiguration<IdentityRoleClaim<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityRoleClaim<string>> builder)
        {
            var permission = Permissions.GetAllPermissions();
            var AdminClaims = new List<IdentityRoleClaim<string>>();
            for (int i = 0; i < permission.Count; i++)
            {
                AdminClaims.Add(new IdentityRoleClaim<string>
                {
                    Id = i + 1,
                    RoleId = DefaultRoles.AdminRoleId,
                    ClaimType = Permissions.Type,
                    ClaimValue = permission[i]
                });
            }
            //Defualt Role
            builder.HasData(AdminClaims);
        }
    }
}
