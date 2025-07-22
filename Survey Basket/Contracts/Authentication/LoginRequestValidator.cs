using Survey_Basket.Contracts.Authentication;

namespace Survey_Basket.validations
{
    public class LoginRequestValidator:AbstractValidator<AuthenticationRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Email).NotEmpty()
                .EmailAddress();
            RuleFor(x => x.password).NotEmpty();
               
        }
    }
}
