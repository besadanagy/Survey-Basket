namespace Survey_Basket.Contracts.Question
{
    public class QuestionRequestValidator : AbstractValidator<QuestionRequest>
    {
        public QuestionRequestValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty()
                .Length(3, 1000);

            RuleFor(x => x.Answers)
                .NotNull();
            RuleFor(x => x.Answers)
                .NotEmpty()
                .Must(answers => answers.Count > 1)
                .WithMessage("Question should have At least two answers.")
                .When(x => x.Answers != null);

            RuleFor(x => x.Answers)
                 .Must(answers => answers.Distinct().Count() == answers.Count)
                 .WithMessage("You can't duplicate Answers for the same question.")
                 .When(x => x.Answers != null);


        }
    }
}
