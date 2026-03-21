using Microsoft.EntityFrameworkCore;


namespace TravelApp.Services.Payment.Data
{
    public class PaymentDbContext : DbContext
    {
        public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options) { }

        public DbSet<Models.Payment> Payments => Set<Models.Payment>();
    }
}
