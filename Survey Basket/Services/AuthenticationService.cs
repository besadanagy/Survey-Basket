
namespace Survey_Basket.Services
{
    public class AuthenticationService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtProvider jwtProvider,
        IOptions<JwtOptions> options,
        IEmailSender emailSender,
        IHttpContextAccessor httpContextAccessor,
        ILogger<AuthenticationService> logger,
        EntityContext Context) : IAuthenticationService
    {
        private readonly JwtOptions options = options.Value;
        private readonly UserManager<ApplicationUser> userManager = userManager;
        private readonly SignInManager<ApplicationUser> signInManager = signInManager;
        private readonly IJwtProvider jwtProvider = jwtProvider;
        private readonly IEmailSender emailSender = emailSender;
        private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
        private readonly ILogger<AuthenticationService> logger = logger;
        private readonly EntityContext context =Context;

        public async Task<Result<AuthenticationResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            if (await userManager.FindByEmailAsync(email) is not { } user )
                return Result.Failure<AuthenticationResponse>(UserErrors.InvalidCredentials);
            if (user.IsDisabled )
                return Result.Failure<AuthenticationResponse>(UserErrors.DisabledUser);

            var result = await signInManager.PasswordSignInAsync(user, password, false, true);
            if (result.Succeeded)
            {
                var (userRoles, userPermissions) = await GetUserRolesAndPermission(user, cancellationToken);

                var (token, expirIn) = jwtProvider.GenerateToken(user,userRoles,userPermissions);
                var refreshToken = GenerateRefreshToken();
                var refreshTokenExpiration = DateTime.UtcNow.AddDays(options.RefreshTokenExpirationDays);

                user.RefreshTokens.Add(new RefreshToken
                {
                    Token = refreshToken,
                    ExpirsOn = refreshTokenExpiration
                });

                await userManager.UpdateAsync(user);
                var response = new AuthenticationResponse(user.Id,user.FirstName,user.LastName,user.Email,token,expirIn,refreshToken,refreshTokenExpiration);
                return Result.Success(response);
                    
            }
            var error = result.IsLockedOut 
                ? UserErrors.LockedUser 
                : result.IsNotAllowed 
                ? UserErrors.EmailNotConfirmed 
                : UserErrors.InvalidCredentials;

            return Result.Failure<AuthenticationResponse>(error);
        }

        public async Task<Result<AuthenticationResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
        {
            var userId = jwtProvider.ValidateToken(token);
            if (userId is null)
                return Result.Failure<AuthenticationResponse>(UserErrors.InvalidCredentials);

            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
                return Result.Failure<AuthenticationResponse>(UserErrors.InvalidCredentials);
            if (user.IsDisabled)
                return Result.Failure<AuthenticationResponse>(UserErrors.DisabledUser);
            if (user.LockoutEnd>DateTime.UtcNow)
                return Result.Failure<AuthenticationResponse>(UserErrors.LockedUser);

            var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);
            if (userRefreshToken is null)
                return Result.Failure<AuthenticationResponse>(UserErrors.InvalidCredentials);

            userRefreshToken.RevokedOn = DateTime.UtcNow;
            var (userRoles, userPermissions) = await GetUserRolesAndPermission(user, cancellationToken);

            var (newToken, expirIn) = jwtProvider.GenerateToken(user,userRoles,userPermissions);

            var newRefreshToken = GenerateRefreshToken();
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(options.RefreshTokenExpirationDays);

            user.RefreshTokens.Add(new RefreshToken
            {
                Token = newRefreshToken,
                ExpirsOn = refreshTokenExpiration
            });

            await userManager.UpdateAsync(user);

            return Result.Success(new AuthenticationResponse(
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email,
                newToken,
                expirIn,
                newRefreshToken,
                refreshTokenExpiration
            ));
        }

        public async Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
        {
            var userId = jwtProvider.ValidateToken(token);
            if (userId is null)
                return Result.Failure<AuthenticationResponse>(UserErrors.InvalidCredentials);

            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
                return Result.Failure<AuthenticationResponse>(UserErrors.InvalidCredentials);

            var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);
            if (userRefreshToken is null)
                return Result.Failure<AuthenticationResponse>(UserErrors.InvalidCredentials);

            userRefreshToken.RevokedOn = DateTime.UtcNow;

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return Result.Failure<AuthenticationResponse>(UserErrors.InvalidCredentials);
                    
            return Result.Success();
        }
        public async Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
        {
            var EmailIsExist=await userManager.Users.AnyAsync(x=>x.Email==request.Email,cancellationToken);
            if (EmailIsExist)
                return Result.Failure<AuthenticationResponse>(UserErrors.DublicatedEmail);

            var user =request.Adapt<ApplicationUser>();
            var result = await userManager.CreateAsync(user, request.Password);
            if(result.Succeeded)
            {
                var code= await userManager.GenerateEmailConfirmationTokenAsync(user);
                code=WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                logger.LogInformation("Email Confirmation Code : {code}",code);

                await SendConfiramtionEmail(user, code);

                return Result.Success();
            }
            var error = result.Errors.FirstOrDefault();
                return Result.Failure<AuthenticationResponse>(new Error (error!.Code,error.Description,StatusCodes.Status400BadRequest));
            
        }
        public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request)
        {
            if(await userManager.FindByIdAsync(request.UserId) is not { } user)
                return Result.Failure<AuthenticationResponse>(UserErrors.InvalidCredentials);
            if(user.EmailConfirmed)
                return Result.Failure<AuthenticationResponse>(UserErrors.EmailIsConfirmed);
            var code = request.Code;
            try
            {
                code= Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
            }
            catch (FormatException)
            {

                return Result.Failure(UserErrors.InvalidCode);
            }
            var result = await userManager.ConfirmEmailAsync(user, code);
            if(result.Succeeded)
             {
                await userManager.AddToRoleAsync(user, DefaultRoles.member);
                return Result.Success(); 
            }

            var error = result.Errors.FirstOrDefault(); 
            return Result.Failure(new Error(error!.Code ,error.Description,StatusCodes.Status400BadRequest));
        }
        public async Task<Result> ResendConfirmationEmailAsync(ResendConfirmationEmailRequest request)
        {
            if (await userManager.FindByEmailAsync(request.Email) is not { } user)
                return Result.Success();
            if (user.EmailConfirmed)
                return Result.Failure<AuthenticationResponse>(UserErrors.EmailIsConfirmed);

            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            logger.LogInformation("Email Confirmation Code : {code}", code);

            await SendConfiramtionEmail(user, code);

            return Result.Success();
        }
        public async Task<Result> SendResetPasswordCodeAsync(string email)
        {
            if (await userManager.FindByEmailAsync(email) is not { } user)
                return Result.Success();
            if(!user.EmailConfirmed)
                return Result.Failure(UserErrors.EmailNotConfirmed);
            var code = await userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            logger.LogInformation("Reset Code : {code}", code);

            await SendResetPasswordEmail(user, code);

            return Result.Success();
        }

        public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if(user is null||!user.EmailConfirmed)
                return Result.Failure(UserErrors.InvalidCode);
            IdentityResult result;
            try
            {
                var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));
                result=await userManager.ResetPasswordAsync(user, code,request.NewPassword);
            }
            catch (FormatException)
            {
                result = IdentityResult.Failed(userManager.ErrorDescriber.InvalidToken());
            }
            if(result.Succeeded)
                return Result.Success();


            var error = result.Errors.First();
            return Result.Failure(new Error(error!.Code, error.Description, StatusCodes.Status401Unauthorized));


        }

        private static string GenerateRefreshToken() =>
            Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        private async Task SendConfiramtionEmail(ApplicationUser user,string code)
        {
            var origin = httpContextAccessor.HttpContext?.Request.Headers.Origin;
            var emailBody = EmailBodyBuilder.generateEmailBody("EmailConfirmation",
                templateModel: new Dictionary<string, string>
                {
                        {"{{name}}",$"{user.FirstName} {user.LastName}" },
                        {"{{action_url}}", $"{origin}/Authentication/ConfirmEmail?UserId={user.Id}&Code={code}"}
                });
            BackgroundJob.Enqueue(()=> emailSender.SendEmailAsync(user.Email!, "Survey Basket : Email Confirmation ✅", emailBody));

            await Task.CompletedTask;
        }
        private async Task SendResetPasswordEmail(ApplicationUser user,string code)
        {
            var origin = httpContextAccessor.HttpContext?.Request.Headers.Origin;
            var emailBody = EmailBodyBuilder.generateEmailBody("ForgetPassword",
                templateModel: new Dictionary<string, string>
                {
                        {"{{name}}",$"{user.FirstName} {user.LastName}" },
                        {"{{action_url}}", $"{origin}/Authentication/ForgetPassword?UserId={user.Email}&Code={code}"}
                });
            BackgroundJob.Enqueue(()=> emailSender.SendEmailAsync(user.Email!, "Survey Basket : Change Password ✅", emailBody));

            await Task.CompletedTask;
        }
        private async Task<(IEnumerable<string> roles, IEnumerable<string> permissions)> GetUserRolesAndPermission(ApplicationUser user,CancellationToken cancellationToken)
        {
            var userRoles= await userManager.GetRolesAsync(user);
            //var userPermissions = await context.Roles
            //    .Join(context.RoleClaims,
            //        role => role.Id,
            //        claim => claim.RoleId,
            //        (role, claim) => new { role, claim })
            //    .Where(x => userRoles.Contains(x.role.Name!))
            //    .Select(x => x.claim.ClaimValue!)
            //    .Distinct()
            //      .ToListAsync(cancellationToken);

            var userPermissions =await(from role in context.Roles
                                       join claim in context.RoleClaims
                                       on role.Id equals claim.RoleId
                                       where userRoles.Contains(role.Name!)
                                       select claim.ClaimValue!)
                                       .Distinct()
                                       .ToListAsync(cancellationToken);

            return (userRoles, userPermissions);

        }
    }
}
