namespace Survey_Basket.Contracts.User
{
    public record ChangePasswordRequest
    (
        string CurrentPassword,
        string NewPassword
    );
}