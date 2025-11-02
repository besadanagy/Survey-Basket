namespace Survey_Basket.Contracts.Authentication
{
    public interface IJwtProvider
    {
        public (string token,int expireIn) GenerateToken(ApplicationUser applicationUser, IEnumerable<string> roles, IEnumerable<string> permissions);
    string? ValidateToken(string token);
    }
}
