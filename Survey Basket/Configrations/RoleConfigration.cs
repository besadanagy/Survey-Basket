
namespace Survey_Basket.validations
{
    public class RoleConfigration : IEntityTypeConfiguration<ApplicationRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationRole> builder)
        {

            //Defualt Role
            builder.HasData([new ApplicationRole
            {
                Id=DefaultRoles.AdminRoleId,
                Name=DefaultRoles.Admin,
                NormalizedName=DefaultRoles.Admin.ToUpper(),
                ConcurrencyStamp=DefaultRoles.AdminRoleConcurrencyStamp
            },
            new ApplicationRole
            {
                Id=DefaultRoles.memberRoleId,
                Name = DefaultRoles.member,
                NormalizedName = DefaultRoles.member.ToUpper(),
                ConcurrencyStamp = DefaultRoles.memberRoleConcurrencyStamp,
                IsDefualt=true

            }]);
        }
    }
}
