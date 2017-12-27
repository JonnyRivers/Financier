using Microsoft.EntityFrameworkCore;
using System;

namespace Financier.Data
{
    public class FinancierDbContext : DbContext
    {
        public FinancierDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }
}
