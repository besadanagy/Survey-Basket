namespace Survey_Basket.Services
{
    public interface IAuthenticationService
    {

        Task<Result<AuthenticationResponse>> GetTokenAsync(string email, string password,CancellationToken cancellationToken=default);
        Task<Result<AuthenticationResponse>> GetRefreshTokenAsync(string token, string RefreshToken,CancellationToken cancellationToken=default);
        Task<Result> RevokeRefreshTokenAsync(string token, string RefreshToken,CancellationToken cancellationToken=default);
        Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
        Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request);
        Task<Result> ResendConfirmationEmailAsync(ResendConfirmationEmailRequest request);
        Task<Result> SendResetPasswordCodeAsync(string email);
        Task<Result> ResetPasswordAsync(ResetPasswordRequest request);

    }
}
