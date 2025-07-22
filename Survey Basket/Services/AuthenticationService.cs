namespace Survey_Basket.Services
{
    public class AuthenticationService(
        UserManager<ApplicationUser> userManager,
        IJwtProvider jwtProvider,
        IOptions<JwtOptions> options) : IAuthenticationService
    {
        private readonly JwtOptions _options = options.Value;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IJwtProvider _jwtProvider = jwtProvider;

        public async Task<Result<AuthenticationResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, password))
                return Result.Failure<AuthenticationResponse>(UserErrors.InvalidCredentials);

            var (token, expirIn) = _jwtProvider.GenerateToken(user);
            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(_options.RefreshTokenExpirationDays);

            user.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                ExpirsOn = refreshTokenExpiration
            });

            await _userManager.UpdateAsync(user);

            return Result.Success(
                new AuthenticationResponse(
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email,
                token,
                expirIn,
                refreshToken,
                refreshTokenExpiration
            ));
        }

        public async Task<Result<AuthenticationResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
        {
            var userId = _jwtProvider.ValidateToken(token);
            if (userId is null)
                return Result.Failure<AuthenticationResponse>(UserErrors.InvalidCredentials);

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return Result.Failure<AuthenticationResponse>(UserErrors.InvalidCredentials);

            var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);
            if (userRefreshToken is null)
                return Result.Failure<AuthenticationResponse>(UserErrors.InvalidCredentials);

            userRefreshToken.RevokedOn = DateTime.UtcNow;
            var (newToken, expirIn) = _jwtProvider.GenerateToken(user);

            var newRefreshToken = GenerateRefreshToken();
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(_options.RefreshTokenExpirationDays);

            user.RefreshTokens.Add(new RefreshToken
            {
                Token = newRefreshToken,
                ExpirsOn = refreshTokenExpiration
            });

            await _userManager.UpdateAsync(user);

            return Result.Success( new AuthenticationResponse(
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
            var userId = _jwtProvider.ValidateToken(token);
            if (userId is null)
                return Result.Failure<AuthenticationResponse>(UserErrors.InvalidCredentials);

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return Result.Failure<AuthenticationResponse>(UserErrors.InvalidCredentials);

            var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);
            if (userRefreshToken is null)
                return Result.Failure<AuthenticationResponse>(UserErrors.InvalidCredentials);

            userRefreshToken.RevokedOn = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return Result.Failure<AuthenticationResponse>(UserErrors.InvalidCredentials);
                    
            return Result.Success();
        }

        private static string GenerateRefreshToken() =>
            Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
}
