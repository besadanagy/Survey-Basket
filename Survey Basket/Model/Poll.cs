namespace Survey_Basket.Model
{
    public class Poll: AuditTable
    {
        public int Id { get; set; }
        public string Title { get; set; }=string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsPublished { get; set; }
        public DateOnly StartsAt { get; set; }
        public DateOnly EndsAt { get;set; }

    }
}
