
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace Survey_Basket.Services
{
    public class UserService(UserManager<ApplicationUser> userManager,
        IRoleService roleService,
        EntityContext context) : IUserService
    {
        private readonly UserManager<ApplicationUser> userManager = userManager;
        private readonly IRoleService roleService = roleService;
        private readonly EntityContext context = context;

        public async Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken)=>
            await(from u in context.Users
                  join ur in context.UserRoles
                  on u.Id equals ur.UserId
                  join r in context.Roles
                  on ur.RoleId equals r.Id into roles
                  where !roles.Any(x=>x.Name==DefaultRoles.member)
                      select new
                      {
                          u.Id,
                          u.FirstName,
                          u.LastName,
                          u.Email,
                          u.IsDisabled,
                          Roles=roles.Select(x => x.Name!).ToList()
                      }
                  )
            .Where(x=>x.IsDisabled == false)
                    .GroupBy(u => new {u.Id,u.FirstName,u.LastName,u.Email,u.IsDisabled})
                    .Select(x=>new UserResponse
                   (
                             x.Key.Id,
                             x.Key.FirstName,
                             x.Key.LastName,
                             x.Key.Email,
                             x.Key.IsDisabled,
                       x.SelectMany(r => r.Roles).Distinct().ToList()
                    ))
                .ToListAsync(cancellationToken);

        public async Task<Result<UserResponse>> GetAsync(string id)
        {
            if (await userManager.FindByIdAsync(id) is not { } user)
                return Result.Failure<UserResponse>(UserErrors.UserNotFound);
            var userRoles= await userManager.GetRolesAsync(user);
            var response = (user, userRoles).Adapt<UserResponse>();
            return Result.Success(response);
        }
        public async Task<Result<UserResponse>> AddAsync(CreateUserRequest request,CancellationToken cancellationToken)
        {
            if(await userManager.Users.AnyAsync(x=>x.Email==request.Email,cancellationToken))
                return Result.Failure<UserResponse>(UserErrors.DublicatedEmail);

            
            var allowedRoles = await roleService.GetAllAsync(cancellationToken: cancellationToken);
            if(request.Roles.Except(allowedRoles.Select(x=>x.Name)).Any())
                return Result.Failure<UserResponse>(UserErrors.InvalidRole);

            var user = request.Adapt<ApplicationUser>();

           
            var result = await userManager.CreateAsync(user);

            if (result.Succeeded)
            {
                await userManager.AddToRolesAsync(user, request.Roles);
                var response = (user, request.Roles).Adapt<UserResponse>();
                return Result.Success(response);
            }

            var error = result.Errors.First();
            return Result.Failure<UserResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }
        public async Task<Result> UpdateAsync(string id,UpdateUserRequest request,CancellationToken cancellationToken)
        {
            if(await userManager.Users.AnyAsync(x=>x.Email==request.Email&&x.Id!=id,cancellationToken))
                return Result.Failure(UserErrors.DublicatedEmail);

            
            var allowedRoles = await roleService.GetAllAsync(cancellationToken: cancellationToken);
            if(request.Roles.Except(allowedRoles.Select(x=>x.Name)).Any())
                return Result.Failure(UserErrors.InvalidRole);

            if(await userManager.FindByIdAsync(id) is not { } user)
                return Result.Failure(UserErrors.UserNotFound);

            user = request.Adapt(user);


            var result = await userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                await context.UserRoles
                    .Where(x=>x.UserId==id)
                    .ExecuteDeleteAsync(cancellationToken);

                await userManager.AddToRolesAsync(user, request.Roles);
                return Result.Success();
            }

            var error = result.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }
        public async Task<Result> ToggleStatusAsync(string id)
        {
            if (await userManager.FindByIdAsync(id) is not { } user)
                return Result.Failure(UserErrors.UserNotFound);

            user.IsDisabled = !user.IsDisabled;

           var result= await userManager.UpdateAsync(user);

            if (result.Succeeded)
                     return Result.Success();

            var error = result.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }

        public async Task<Result> UnLockAsync(string id)
        {
            if (await userManager.FindByIdAsync(id) is not { } user)
                return Result.Failure(UserErrors.UserNotFound);
            var result = await userManager.SetLockoutEndDateAsync(user,null);

            if (result.Succeeded)
                return Result.Success();

            var error = result.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }
        public async Task<Result<UserProfileResponse>> GetProfileAsync(string userId)
        {
            var user = await userManager.Users
                .Where(x => x.Id == userId)
                .ProjectToType<UserProfileResponse>()
                .SingleAsync();
            return Result.Success(user);
        }
        public async Task<Result> UpdateProfileAsync(string userId, UpdateProfileRequest request)
        {
            //var user = await userManager.FindByIdAsync(userId);
            //user = request.Adapt(user);
            //await userManager.UpdateAsync(user!);

            await userManager.Users
                .Where(x => x.Id == userId)
                .ExecuteUpdateAsync(setter =>
                setter
                .SetProperty(x => x.FirstName, request.FirstName)
                .SetProperty(x => x.LastName, request.LastName)
                );

            return Result.Success();
        }
        public async Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request)
        {
            var user = await userManager.FindByIdAsync(userId);
            var result = await userManager.ChangePasswordAsync(user!, request.CurrentPassword, request.NewPassword);
            if (result.Succeeded)
                return Result.Success();

            var error = result.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));

        }
    }
}
