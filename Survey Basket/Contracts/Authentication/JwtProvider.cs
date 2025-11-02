using System.Text.Json;

namespace Survey_Basket.Contracts.Authentication
{
    public class JwtProvider(IOptions<JwtOptions> options) : IJwtProvider
    {
        public JwtOptions Options = options.Value;

        public (string token, int expireIn) GenerateToken(ApplicationUser user,IEnumerable<string> roles,IEnumerable<string> permissions)
        {
            Claim[] claims = [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName),
                new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(nameof(roles), JsonSerializer.Serialize(roles),JsonClaimValueTypes.JsonArray),
                new Claim(nameof(permissions), JsonSerializer.Serialize(permissions),JsonClaimValueTypes.JsonArray)
                 ];

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Options.Key)); // Replace with your secret key
            var singer = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: Options.issuer,
                audience: Options.audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Options.expiresIn), // Token expiration time
                signingCredentials: singer
            );
            return (token: new JwtSecurityTokenHandler().WriteToken(token), expireIn: Options.expiresIn * 60);
        }

        public string? ValidateToken(string token)
        {
            var tokenHandler=new JwtSecurityTokenHandler();
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Options.Key)); // Replace with your secret key

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    IssuerSigningKey= symmetricSecurityKey,
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ClockSkew=TimeSpan.Zero
                }, out SecurityToken validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;
                return jwtToken.Claims.First(x=>x.Type==JwtRegisteredClaimNames.Sub).Value;
            }
            catch
            {
                return null;
            }
        }
    }
}
