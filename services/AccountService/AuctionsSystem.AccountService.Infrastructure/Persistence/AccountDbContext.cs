using AuctionsSystem.AccountService.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace AuctionsSystem.AccountService.Infrastructure.Persistence
{
    public class AccountDbContext : DbContext
    {
        public AccountDbContext(DbContextOptions<AccountDbContext> options) : base(options) { }

        public DbSet<Account> Accounts => Set<Account>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AccountDbContext).Assembly);
        }

    }
}
