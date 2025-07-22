namespace Survey_Basket.Errors
{
    public class PollErorrs
    {
        public static readonly Error PollNotFound = new("Poll.NotFound", "No Poll was found with Given ID.");
    }

}