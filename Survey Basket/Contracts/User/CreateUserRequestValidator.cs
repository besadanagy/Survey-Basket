namespace Survey_Basket.Contracts.User
{
    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestValidator()
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
                .Length(3, 100);
            RuleFor(x => x.LastName)
                .NotEmpty()
                .Length(3, 100);
            RuleFor(x => x.Roles)
               .NotEmpty()
               .NotNull()
               .WithMessage("Role are required.");

            RuleFor(x => x.Roles)
                  .Must(x => x.Distinct().Count() == x.Count())
                  .WithMessage("You can't add duplicate role for the same user");
            //.When(x=>x.Permissions!=null);
        }
    }
}
