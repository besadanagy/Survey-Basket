
namespace Survey_Basket.Contracts.Question
{
    public record QuestionResponse(
        int Id,
        string Content,
        IEnumerable<AnswerResponse> Answers);
}
