namespace Survey_Basket.Contracts.Polls
{
    public record PollRequest
   (
        string Title,
        string Description,
        DateOnly StartsAt,
        DateOnly EndsAt
    );
}
