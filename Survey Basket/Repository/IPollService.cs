
namespace Survey_Basket.Repository
{
    public interface IPollService
    {
        public Poll CreatePoll(Poll poll);
        public List<Poll> GetAllPolls();
        public Poll GetPollById(int id);
        public bool UpdatePoll(Poll NewPoll);
        public bool DeletePoll(int id);

    }
}
