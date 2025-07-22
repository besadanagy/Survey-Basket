
namespace Survey_Basket.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthenticationController(IAuthenticationService authenticationService) : ControllerBase
    {
        public IAuthenticationService AuthenticationService { get; } = authenticationService;



        [HttpPost("")]
        public async Task<IActionResult> LoginAsync(AuthenticationRequest Request, CancellationToken cancellationToken)
        {
            var authResult = await AuthenticationService.GetTokenAsync(Request.Email, Request.password, cancellationToken);

            return authResult.IsFailure ?
                authResult.ToProblem(StatusCodes.Status403Forbidden)
                : Ok(authResult);
        }
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshTokenAsync(RefreshTokenRequest Request, CancellationToken cancellationToken)
        {
            var authResult = await AuthenticationService.GetRefreshTokenAsync(Request.Token, Request.RefreshToken, cancellationToken);
            return authResult.IsFailure ?
                authResult.ToProblem(StatusCodes.Status403Forbidden)
                : Ok(authResult);
        }
        [HttpPut("Revoke-Refresh-Token")]
        public async Task<IActionResult> RevokeRefreshTokenAsync(RefreshTokenRequest Request, CancellationToken cancellationToken)
        {
            var isRevoked = await AuthenticationService.RevokeRefreshTokenAsync(Request.Token, Request.RefreshToken, cancellationToken);
            return isRevoked.IsFailure ?
                 isRevoked.ToProblem(StatusCodes.Status403Forbidden)
                 : Ok(isRevoked);
        }
    }
}