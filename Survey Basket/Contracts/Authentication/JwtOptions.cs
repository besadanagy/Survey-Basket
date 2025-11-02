
namespace Survey_Basket.Contracts.Authentication
{
    public class JwtOptions
    {
        public static string SectionName { get; set; } = "JWT";
        [Required]
        public string Key { get; set; } = string.Empty;
        [Required]
        public string issuer { get; set; } = string.Empty;
        [Required]
        public string audience { get; set; } = string.Empty;
        [Range(1, int.MaxValue)]
        public int expiresIn { get; set; }
        [Range(1, int.MaxValue)]
        public int RefreshTokenExpirationDays { get; set; }
         
    }
}
