namespace Survey_Basket.Services
{
    public class NotificationService(
        EntityContext context,
        UserManager<ApplicationUser> userManager,
        IHttpContextAccessor httpContextAccessor,
        IEmailSender emailSender) : INotificationService
    {
        private readonly EntityContext context = context;
        private readonly UserManager<ApplicationUser> userManager = userManager;
        private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
        private readonly IEmailSender emailSender = emailSender;

        public async Task SendNewPollNotifiaction(int? PollId = null)
        {
            IEnumerable<Poll> Polls = [];
            if (PollId.HasValue)
            {
                var poll = await context.Polls.SingleOrDefaultAsync(p => p.Id == PollId && p.IsPublished);
                Polls = [poll!];
            }
            else
            {
                Polls = await context.Polls
                    .Where(p => p.IsPublished && p.StartsAt == DateOnly.FromDateTime(DateTime.UtcNow))
                    .AsNoTracking()
                    .ToListAsync();
            }
            //TODO:Select Member Only
            var users = await userManager.Users.ToListAsync();
            var origin = httpContextAccessor.HttpContext?.Request.Headers.Origin;

            foreach (var poll in Polls)
            {
                foreach (var user in users)
                {
                    var placeHolders = new Dictionary<string, string>
                    {
                        {"{{name}}",$"{user.FirstName} {user.LastName}" },
                        {"{{pollTill}}",poll.Title },
                        {"{{endDate}}",poll.EndsAt.ToString() },
                        {"{{url}}", $"{origin}/Polls/start/{poll.Id}"}
                    };

                    var body = EmailBodyBuilder.generateEmailBody("PollNotification", placeHolders);
                    await emailSender.SendEmailAsync(user.Email!, $"Survey Basket : New Poll Available 📣 - {poll.Id}", body);
                }
            }
        }
    }
}
