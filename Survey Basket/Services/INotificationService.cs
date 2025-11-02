namespace Survey_Basket.Services
{
    public interface INotificationService
    {
        Task SendNewPollNotifiaction(int? PollId=null);
    }
}
