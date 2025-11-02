
namespace Survey_Basket.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [EnableRateLimiting("iplimit")]
    public class AuthenticationController(IAuthenticationService authenticationService) : ControllerBase
    {
        public IAuthenticationService AuthenticationService { get; } = authenticationService;
        [HttpPost("")]
        public async Task<IActionResult> LoginAsync(AuthenticationRequest Request, CancellationToken cancellationToken)
        {
            Thread.Sleep(6000);
            var authResult = await AuthenticationService.GetTokenAsync(Request.Email, Request.password, cancellationToken);

            return authResult.IsSuccess ?
                 Ok(authResult)
                : authResult.ToProblem();
        }
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshTokenAsync(RefreshTokenRequest Request, CancellationToken cancellationToken)
        {
            var authResult = await AuthenticationService.GetRefreshTokenAsync(Request.Token, Request.RefreshToken, cancellationToken);
            return authResult.IsSuccess ?
                Ok(authResult)
                : authResult.ToProblem();
        }
        [HttpPost("Revoke-Refresh-Token")]
        public async Task<IActionResult> RevokeRefreshTokenAsync(RefreshTokenRequest Request, CancellationToken cancellationToken)
        {
            var isRevoked = await AuthenticationService.RevokeRefreshTokenAsync(Request.Token, Request.RefreshToken, cancellationToken);
            return isRevoked.IsSuccess ?
                  Ok()
                  : isRevoked.ToProblem();
        }
        [HttpPost("Register")]
        [DisableRateLimiting]
        public async Task<IActionResult> Register(RegisterRequest Request, CancellationToken cancellationToken)
        {
            var result = await AuthenticationService.RegisterAsync(Request,  cancellationToken);
            return result.IsSuccess ?
                 Ok()
                : result.ToProblem();
        }
        [HttpPost("Confirm-Email")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailRequest Request)
        {
            var result = await AuthenticationService.ConfirmEmailAsync(Request);
            return result.IsSuccess ?
                 Ok()
                : result.ToProblem();
        }
        [HttpPost("Resend-Confirmation-Email")]
        public async Task<IActionResult> ResendConfirmationEmail(ResendConfirmationEmailRequest Request)
        {
            var result = await AuthenticationService.ResendConfirmationEmailAsync(Request);
            return result.IsSuccess ?
                 Ok()
                : result.ToProblem();
        }
        [HttpPost("Forget-password")]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordRequest Request)
        {
            var result = await AuthenticationService.SendResetPasswordCodeAsync(Request.Email);
            return result.IsSuccess ?
                 Ok()
                : result.ToProblem();
        }
        [HttpPost("Reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest Request)
        {
            var result = await AuthenticationService.ResetPasswordAsync(Request);
            return result.IsSuccess ?
                 Ok()
                : result.ToProblem();
        }

    
    }
}