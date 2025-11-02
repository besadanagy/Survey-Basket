using Survey_Basket.Contracts.Common;
using System.Linq.Dynamic.Core;
namespace Survey_Basket.Services
{
    public class QuestionService(
        EntityContext context,
        HybridCache hybridCache,
        ILogger<QuestionService> logger) : IQuestionService
    {
        private readonly EntityContext _context = context;
        private readonly HybridCache _hybridCache = hybridCache;
        private readonly ILogger<QuestionService> _logger = logger;

        private const string _cachePrefix = "availableQuestions";

        public async Task<Result<PaginatedList<QuestionResponse>>> GetAllAsync(int pollId,RequestFilters filter, CancellationToken cancellationToken = default)
        {
            var pollIsExists = await _context.Polls.AnyAsync(x => x.Id == pollId, cancellationToken);

            if (!pollIsExists)
                return Result.Failure<PaginatedList<QuestionResponse>>(PollErrors.PollNotFound);

            var query = _context.Questions
                .Where(x => x.PollId == pollId );
            if (!string.IsNullOrEmpty(filter.SearchValue))
            {
                query = context.Questions.Where(x => x.Content.Contains(filter.SearchValue));
            }
            if (!string.IsNullOrEmpty(filter.SortColumn))
            {
                query=query.OrderBy($"{filter.SortColumn} {filter.SortDirection}");
            }
            var source = query 
                .Include(x => x.Answers)
                .ProjectToType<QuestionResponse>()
                .AsNoTracking();

            var response= await PaginatedList<QuestionResponse>.CreateAsync(source, filter.PageNumber, filter.PageSize, cancellationToken);

            return Result.Success(response);
        }

        public async Task<Result<IEnumerable<QuestionResponse>>> GetAvailableAsync(int pollId, string userId, CancellationToken cancellationToken = default)
        {
            //var hasVote = await _context.Votes.AnyAsync(x => x.PollId == pollId && x.UserId == userId, cancellationToken);

            //if (hasVote)
            //    return Result.Failure<IEnumerable<QuestionResponse>>(VoteErrors.DuplicatedVote);

            //var pollIsExists = await _context.Polls.AnyAsync(x => x.Id == pollId && x.IsPublished && x.StartsAt <= DateOnly.FromDateTime(DateTime.UtcNow) && x.EndsAt >= DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken);

            //if (!pollIsExists)
            //    return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);

            var cacheKey = $"{_cachePrefix}-{pollId}";
            _logger.LogInformation("Fetching available questions for PollId {PollId} with CacheKey {CacheKey}", pollId, cacheKey);

            var questions = await _hybridCache.GetOrCreateAsync<IEnumerable<QuestionResponse>>(
                cacheKey,
                async cacheEntry =>
                {
                    _logger.LogInformation("Cache MISS for {CacheKey}. Loading questions from database...", cacheKey);

                    var list = await _context.Questions
                        .Where(x => x.PollId == pollId && x.IsActive)
                        .Include(x => x.Answers)
                        .Select(q => new QuestionResponse(
                            q.Id,
                            q.Content,
                            q.Answers.Where(a => a.IsActive).Select(a => new AnswerResponse(a.Id, a.Content))
                        ))
                        .AsNoTracking()
                        .ToListAsync(cancellationToken);

                    return list;
                }
        );

            return Result.Success(questions!);
        }


        public async Task<Result<QuestionResponse>> GetAsync(int pollId, int id, CancellationToken cancellationToken = default)
        {


            var question = await _context.Questions
                .Where(x => x.PollId == pollId && x.Id == id)
                .Include(x => x.Answers)
                .ProjectToType<QuestionResponse>()
                .AsNoTracking()
                .SingleOrDefaultAsync(cancellationToken);

            if (question is null)
                return Result.Failure<QuestionResponse>(QuestionErrors.QuestionNotFound);

            return Result.Success(question);
        }

        public async Task<Result<QuestionResponse>> AddAsync(int pollId, QuestionRequest request, CancellationToken cancellationToken = default)
        {
            var pollIsExists = await _context.Polls.AnyAsync(x => x.Id == pollId, cancellationToken: cancellationToken);

            if (!pollIsExists)
                return Result.Failure<QuestionResponse>(PollErrors.PollNotFound);

            var questionIsExists = await _context.Questions.AnyAsync(x => x.Content == request.Content && x.PollId == pollId, cancellationToken: cancellationToken);

            if (questionIsExists)
                return Result.Failure<QuestionResponse>(QuestionErrors.DuplicatedQuestionContent);

            var question = request.Adapt<Question>();
            question.PollId = pollId;

            await _context.AddAsync(question, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            await _hybridCache.RemoveAsync($"{_cachePrefix}-{pollId}", cancellationToken);

            return Result.Success(question.Adapt<QuestionResponse>());
        }

        public async Task<Result> UpdateAsync(int pollId, int id, QuestionRequest request, CancellationToken cancellationToken = default)
        {
            var questionIsExists = await _context.Questions
                .AnyAsync(x => x.PollId == pollId
                    && x.Id != id
                    && x.Content == request.Content,
                    cancellationToken
                );

            if (questionIsExists)
                return Result.Failure(QuestionErrors.DuplicatedQuestionContent);

            var question = await _context.Questions
                .Include(x => x.Answers)
                .SingleOrDefaultAsync(x => x.PollId == pollId && x.Id == id, cancellationToken);

            if (question is null)
                return Result.Failure(QuestionErrors.QuestionNotFound);

            question.Content = request.Content;

            //current answers
            var currentAnswers = question.Answers.Select(x => x.Content).ToList();

            //add new answer
            var newAnswers = request.Answers.Except(currentAnswers).ToList();

            newAnswers.ForEach(answer =>
            {
                question.Answers.Add(new Answer { Content = answer });
            });

            question.Answers.ToList().ForEach(answer =>
            {
                answer.IsActive = request.Answers.Contains(answer.Content);
            });

            await _context.SaveChangesAsync(cancellationToken);

            await _hybridCache.RemoveAsync($"{_cachePrefix}-{pollId}", cancellationToken);

            return Result.Success();
        }

        public async Task<Result> ToggleStatusAsync(int pollId, int id, CancellationToken cancellationToken = default)
        {
            var question = await _context.Questions.SingleOrDefaultAsync(x => x.PollId == pollId && x.Id == id, cancellationToken);

            if (question is null)
                return Result.Failure(QuestionErrors.QuestionNotFound);

            question.IsActive = !question.IsActive;

            await _context.SaveChangesAsync(cancellationToken);

            await _hybridCache.RemoveAsync($"{_cachePrefix}-{pollId}", cancellationToken);

            return Result.Success(question);
        }
    }
}
