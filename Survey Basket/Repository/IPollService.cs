namespace Survey_Basket.Repository
{
    public interface IPollService
    {
        Task<Result<List<PollResponse>>> GetAll();
        Task<Result<PollResponse>> GetById(int id);
        Task<Result<PollResponse>> Create(PollRequest poll, string userId);
        Task<Result<PollResponse>> Update(int Id,PollRequest NewPoll);
        Task<Result> Delete(int id);

    }
}
