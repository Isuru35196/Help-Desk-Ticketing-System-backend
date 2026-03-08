namespace HelpDesk.API.Data
{
    using HelpDesk.API.Models;
    using Microsoft.EntityFrameworkCore;

    public class HelpDeskDbContext : DbContext
    {
        public HelpDeskDbContext(DbContextOptions<HelpDeskDbContext> options) : base(options)
        {
        }

        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships and constraints if needed
            // For example, if you have a User-Ticket relationship:
            // modelBuilder.Entity<Ticket>()
            //     .HasOne(t => t.User)
            //     .WithMany()
            //     .HasForeignKey(t => t.UserId);
        }
    }
}