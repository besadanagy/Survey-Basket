namespace Survey_Basket.Contracts.Authentication
{
    public interface IJwtProvider
    {
        public (string token,int expireIn) GenerateToken(ApplicationUser applicationUser);
    string? ValidateToken(string token);
    }
}
