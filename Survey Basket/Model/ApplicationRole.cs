namespace Survey_Basket.Model
{
    public class ApplicationRole:IdentityRole
    {
        public bool IsDefualt { get; set; }
        public bool IsDeleted { get; set; }

    }
}
