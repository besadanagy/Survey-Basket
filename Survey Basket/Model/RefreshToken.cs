namespace Survey_Basket.Model
{
    [Owned]
    public class RefreshToken
    {
        public string Token { get; set; }=string.Empty;
        public DateTime ExpirsOn { get; set; }
        public DateTime CreatedOn { get; set; }=DateTime.UtcNow;
        public DateTime? RevokedOn { get; set; }
        public bool IsExpired =>DateTime.UtcNow >= ExpirsOn;
        public bool IsActive => RevokedOn is null && !IsExpired;
    }
}
