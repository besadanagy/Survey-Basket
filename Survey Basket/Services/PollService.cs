
namespace Survey_Basket.Services
{
    public class PollService:IPollService
    {
        private readonly EntityContext _context;

        public PollService(EntityContext context)
        {
            _context = context;
        }

        // CREATE
        public Poll CreatePoll(Poll poll)
        {
            var Poll = new Poll
            {
                
                Title = poll.Title,
                Description = poll.Description
            };
            _context.Polls.Add(Poll);
            _context.SaveChanges();
            return Poll;
        }

        // READ ALL
        public List<Poll> GetAllPolls()
        {
            return _context.Polls.ToList();
        }

        // READ BY ID
        public Poll GetPollById(int id)
        {
            return _context.Polls.FirstOrDefault(p => p.Id == id);
        }

        // UPDATE
        public bool UpdatePoll(Poll NewPoll)
        {
            var OldPoll = GetPollById(NewPoll.Id);
            if (NewPoll == null)
                return false;

            OldPoll.Title = NewPoll.Title;
            OldPoll.Description = NewPoll.Description;
            _context.SaveChanges();
            return true;
        }

        // DELETE
        public bool DeletePoll(int id)
        {
            var Poll = GetPollById(id);
            if (Poll == null)
                return false;

            _context.Polls.Remove(Poll);
            _context.SaveChanges();
            return true;
        }
    }
}

