namespace Survey_Basket.Contracts.Roles
{
    public class RoleRequestValidator:AbstractValidator<RoleRequest>
    {
        public RoleRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Role name is required.")
                .Length(3,256);

            RuleFor(x => x.Permissions)
                .NotEmpty()
                .NotNull()
                .WithMessage("Permissions are required.");
            RuleFor(x => x.Permissions)
                    .Must(x => x.Distinct().Count() == x.Count())
                    .WithMessage("You can't add duplicate permission for the same Role");
                //.When(x=>x.Permissions!=null);
        }
    }
}
