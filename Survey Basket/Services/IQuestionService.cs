using Survey_Basket.Contracts.Common;

namespace Survey_Basket.Services.Interfaces
{
    public interface IQuestionService
    {
        Task<Result<PaginatedList<QuestionResponse>>> GetAllAsync(int PollId, RequestFilters request, CancellationToken cancellationToken = default);
        Task<Result<QuestionResponse>> GetAsync(int PollId,int id, CancellationToken cancellationToken = default);
        Task<Result<QuestionResponse>> AddAsync(int PollId,QuestionRequest request, CancellationToken cancellationToken = default);
        Task<Result> UpdateAsync(int PollId, int id, QuestionRequest request, CancellationToken cancellationToken = default);
        Task<Result> ToggleStatusAsync(int PollId,int id, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<QuestionResponse>>> GetAvailableAsync(int pollId, string userId, CancellationToken cancellationToken = default);

    }
}
