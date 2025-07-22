namespace Survey_Basket.Services
{
    public class PollService:IPollService
    {
        private readonly EntityContext _context;

        public PollService(EntityContext context)
        {
            _context = context;
        }
        // READ ALL
        public async Task<Result<List<PollResponse>>> GetAll()
        {
            var polls = await _context.Polls.ToListAsync();
            return polls is null ?
               Result.Failure<List<PollResponse>>(PollErorrs.PollNotFound) :
                Result.Success(polls.Adapt<List<PollResponse>>());
        }
        // READ BY ID
        public async Task<Result<PollResponse>> GetById(int id)
        {
            var poll = await _context.Polls.FirstOrDefaultAsync(p => p.Id == id);
            return poll is null ?
              Result.Failure<PollResponse>(PollErorrs.PollNotFound) :
               Result.Success(poll.Adapt<PollResponse>());
        }
        // CREATE
        public async Task<Result<PollResponse>> Create(PollRequest poll, string userId)
        {
            if (poll is null)
                return Result.Failure<PollResponse>(PollErorrs.PollNotFound);

            var entity = new Poll
            {
                Title = poll.Title,
                Description = poll.Description,
                StartsAt = poll.StartsAt,
                EndsAt = poll.EndsAt,
                CreatedById = userId
            };

            await _context.Polls.AddAsync(entity);
            await _context.SaveChangesAsync();

            return Result.Success(entity.Adapt<PollResponse>());
        }




        // UPDATE
        public async Task<Result<PollResponse>> Update(int id, PollRequest newPoll)
        {
            if (newPoll is null)
                return Result.Failure<PollResponse>(PollErorrs.PollNotFound);

            var oldPoll = await _context.Polls.FindAsync(id);
            if (oldPoll is null)
                return Result.Failure<PollResponse>(PollErorrs.PollNotFound);

            oldPoll.Title = newPoll.Title;
            oldPoll.Description = newPoll.Description;
            oldPoll.StartsAt = newPoll.StartsAt;
            oldPoll.EndsAt = newPoll.EndsAt;

            await _context.SaveChangesAsync();

            return Result.Success(oldPoll.Adapt<PollResponse>());
        }

        // DELETE
        public async Task<Result> Delete(int id)
        {
            var poll = await _context.Polls.FindAsync(id);

            if (poll is null)
                return Result.Failure(PollErorrs.PollNotFound);

            _context.Polls.Remove(poll);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

       
    }
}

