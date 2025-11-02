
namespace Survey_Basket.Services
{
    public class RoleService(RoleManager< ApplicationRole> roleManager,
        EntityContext context) : IRoleService
    {
        private readonly RoleManager<ApplicationRole> roleManager = roleManager;
        private readonly EntityContext context = context;

        public async Task<IEnumerable<RoleResponse>> GetAllAsync(bool? includeDisabled = false, CancellationToken cancellationToken=default) =>
            await roleManager.Roles
            .Where(r => !r.IsDefualt && (!r.IsDeleted || (includeDisabled.HasValue && includeDisabled.Value)))
            .ProjectToType<RoleResponse>()
            .ToListAsync(cancellationToken);

        public async Task<Result<RoleDetailsResponse>> GetAsync(string Id)
        {
            if(await roleManager.FindByIdAsync(Id) is not { }role)
                return Result.Failure<RoleDetailsResponse>(RoleErrors.RoleNotFound);

            var permission = await roleManager.GetClaimsAsync(role);
            var response=new RoleDetailsResponse(Id,role.Name!,role.IsDeleted,permission.Select(x=>x.Value));
            return Result.Success(response);
        }

        public async Task<Result<RoleDetailsResponse>> AddAsync(RoleRequest request)
        {
            var roleIsExist=await roleManager.RoleExistsAsync(request.Name);
            if(roleIsExist)
                return Result.Failure<RoleDetailsResponse>(RoleErrors.DublicatedRole);
            var allowedPermissions=Permissions.GetAllPermissions();
            if(request.Permissions.Except(allowedPermissions).Any())
                return Result.Failure<RoleDetailsResponse>(RoleErrors.InvalidPermission);
            var role = new ApplicationRole
            {
                Name = request.Name,
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };
            var result=await roleManager.CreateAsync(role);
            if(result.Succeeded)
            {
                var permission = request.Permissions.Select(p => new IdentityRoleClaim<string>
                {
                    ClaimType = Permissions.Type,
                    ClaimValue = p,
                    RoleId = role.Id
                });
                await context.AddRangeAsync(permission);
                await context.SaveChangesAsync();
                var response=new RoleDetailsResponse(role.Id,role.Name!,role.IsDeleted,request.Permissions);
                return Result.Success(response);
            }
            var error=result.Errors.First();
            return Result.Failure<RoleDetailsResponse>(new Error(error.Code,error.Description,StatusCodes.Status400BadRequest));
        }
        public async Task<Result> UpadteAsync(string id,RoleRequest request)
        {
            if (await roleManager.FindByIdAsync(id) is not { } role)
                return Result.Failure<RoleDetailsResponse>(RoleErrors.RoleNotFound);

            var roleIsExist =await roleManager.Roles.AnyAsync(x=>x.Name==request.Name&&x.Id!=id);
            if(roleIsExist)
                return Result.Failure<RoleDetailsResponse>(RoleErrors.DublicatedRole);

            var allowedPermissions=Permissions.GetAllPermissions();
            if(request.Permissions.Except(allowedPermissions).Any())
                return Result.Failure<RoleDetailsResponse>(RoleErrors.InvalidPermission);
            
            
           role.Name= request.Name;
            var result=await roleManager.UpdateAsync(role);
            if(result.Succeeded)
            {
                var currentPermissions=await context.RoleClaims
                    .Where(x=>x.RoleId==id&&x.ClaimType==Permissions.Type)
                    .Select(x=>x.ClaimValue)
                    .ToListAsync();

                var newPermissions=request.Permissions
                    .Except(currentPermissions)
                    .Select(p => new IdentityRoleClaim<string>
                    {
                        ClaimType = Permissions.Type,
                        ClaimValue = p,
                        RoleId = role.Id
                    });

                var removedPermissions = currentPermissions.Except(request.Permissions);

                await context.RoleClaims
                    .Where(x=>x.RoleId==id&&removedPermissions.Contains(x.ClaimValue))
                    .ExecuteDeleteAsync();

                await context.AddRangeAsync(newPermissions);
                await context.SaveChangesAsync();
                return Result.Success();
            }
            var error=result.Errors.First();
            return Result.Failure<RoleDetailsResponse>(new Error(error.Code,error.Description,StatusCodes.Status400BadRequest));
        }
        public async Task<Result> ToggleStatusAsync(string id)
        {
            if (await roleManager.FindByIdAsync(id) is not { } role)
                return Result.Failure<RoleDetailsResponse>(RoleErrors.RoleNotFound);

            role.IsDeleted = !role.IsDeleted;

            await roleManager.UpdateAsync(role);

            return Result.Success();
        }
    }
}
