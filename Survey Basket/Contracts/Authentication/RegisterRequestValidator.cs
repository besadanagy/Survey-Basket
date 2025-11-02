using Survey_Basket.Abstractions.Consts;
using System.Text.RegularExpressions;

namespace Survey_Basket.Contracts.Authentication
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();
            RuleFor(x => x.Password)
                .Matches(RegexPatterns.PasswordPattern)
                .NotEmpty()
                .WithMessage("Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one digit, and one special character.");
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .Length(3,100);
            RuleFor(x => x.LastName)
                .NotEmpty()
                .Length(3, 100);



        }

    }
}
