using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Financier.Data
{
    public class FinancierDbContext : DbContext
    {
        public FinancierDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // TODO: this is a sledghammer to crack a nut.  We want to just prevent cascading deletes when
            // AccountRelationship records are deleted.
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountRelationship> AccountRelationships { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }
}
