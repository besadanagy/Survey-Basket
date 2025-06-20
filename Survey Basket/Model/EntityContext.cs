using Microsoft.EntityFrameworkCore;

namespace Survey_Basket.Model
{
    public class EntityContext : DbContext
    {
        public DbSet<Poll> Polls { get; set; }

        public EntityContext(DbContextOptions options) : base(options)
        {
        }

        public EntityContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=Survey Basket;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");
            base.OnConfiguring(optionsBuilder);

        }
    }
}
