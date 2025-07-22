
namespace Survey_Basket.Repository
{
    public interface IAuthenticationService
    {

        public Task<Result<AuthenticationResponse>> GetTokenAsync(string email, string password,CancellationToken cancellationToken=default);
        public Task<Result<AuthenticationResponse>> GetRefreshTokenAsync(string token, string RefreshToken,CancellationToken cancellationToken=default);
        public Task<Result> RevokeRefreshTokenAsync(string token, string RefreshToken,CancellationToken cancellationToken=default);
       
    }
}
