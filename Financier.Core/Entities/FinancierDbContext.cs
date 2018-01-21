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
            // These are required for SQL Server.
            // Terrifyingly, SQLite doesn't seem to care!

            // WithMany() is required as each Account could appear twice in any AccountRelationship
            // WithMany() has no lambda because there is no matching navigation property in Account
            // We use DeleteBehavior.Restrict to prevent attempts to delete an Account that has 
            // any AccountRelationship references, which would break refential integrity
            modelBuilder.Entity<AccountRelationship>()
                .HasOne(ar => ar.SourceAccount)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<AccountRelationship>()
                .HasOne(ar => ar.DestinationAccount)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            // WithMany() is required as each Account could appear twice in any BudgetTransaction
            // WithMany() has no lambda because there is no matching navigation property in Account
            // We use DeleteBehavior.Restrict to prevent attempts to delete an Account that has 
            // any BudgetTransaction references, which would break refential integrity
            modelBuilder.Entity<BudgetTransaction>()
                .HasOne(ar => ar.CreditAccount)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<BudgetTransaction>()
                .HasOne(ar => ar.DebitAccount)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);


            // WithMany() is required as each Account could appear twice in any Transaction
            // WithMany() has no lambda because there is no matching navigation property in Account
            // We use DeleteBehavior.Restrict to prevent attempts to delete an Account that has 
            // any Transaction references, which would break refential integrity
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
