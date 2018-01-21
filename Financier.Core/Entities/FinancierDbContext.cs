using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Financier.Entities
{
    public class FinancierDbContext : DbContext
    {
        public FinancierDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccountRelationship>()
                .HasOne(ar => ar.SourceAccount)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AccountRelationship>()
                .HasOne(ar => ar.DestinationAccount)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BudgetTransaction>()
                .HasOne(ar => ar.CreditAccount)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BudgetTransaction>()
                .HasOne(ar => ar.DebitAccount)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasOne(ar => ar.CreditAccount)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasOne(ar => ar.DebitAccount)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountRelationship> AccountRelationships { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<BudgetTransaction> BudgetTransactions { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }
}
