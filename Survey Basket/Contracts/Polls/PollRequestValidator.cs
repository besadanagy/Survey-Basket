
namespace Survey_Basket.Contracts.Polls
{
    public class PollRequestValidator:AbstractValidator<PollRequest>
    {
        public PollRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("please add a {PropertyName}")
                .Length(3,100)
                .WithMessage("{PropertyName} must be at least {MinLength} and Maximum {MaxLength}" +
                ",you entred {TotalLength} and PropertyValue: {PropertyValue} add PropertyPath: {PropertyPath}");

        }
    }
}
