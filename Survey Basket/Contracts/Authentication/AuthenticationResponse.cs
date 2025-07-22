namespace Survey_Basket.Contracts.Authentication
{
    public record AuthenticationResponse(
        string Id,
        string FirstName,
        string LastName,
        string? Email,
        string Token,
        int ExpiresIn ,
        string RefreshToken,
        DateTime RefreshTokenExpiration
        );
    
}
