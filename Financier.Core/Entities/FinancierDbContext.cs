using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Financier.Entities
{
    public class FinancierDbContext : DbContext
    {
        public FinancierDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountRelationship> AccountRelationships { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<BudgetTransaction> BudgetTransactions { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }
}
