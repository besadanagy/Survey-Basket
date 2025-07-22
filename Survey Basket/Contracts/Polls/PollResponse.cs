namespace Survey_Basket.Contracts.Polls
{
    public record PollResponse(
        int Id,
        string Information,
        bool IsPublished,
        DateOnly StartsAt,
        DateOnly EndsAt
    );
}
