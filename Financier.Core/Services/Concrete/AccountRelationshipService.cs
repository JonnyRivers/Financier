using System;
using System.Collections.Generic;
using System.Linq;
using Financier.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Financier.Services
{
    public class AccountRelationshipService : IAccountRelationshipService
    {
        private ILogger<AccountRelationshipService> m_logger;
        private FinancierDbContext m_dbContext;

        public AccountRelationshipService(ILogger<AccountRelationshipService> logger, FinancierDbContext dbContext)
        {
            m_logger = logger;
            m_dbContext = dbContext;
        }

        public void Create(AccountRelationship accountRelationship)
        {
            var entity = new Entities.AccountRelationship
            {
                SourceAccountId = accountRelationship.SourceAccount.AccountId,
                DestinationAccountId = accountRelationship.DestinationAccount.AccountId,
                Type = accountRelationship.Type
            };

            m_dbContext.AccountRelationships.Add(entity);
            m_dbContext.SaveChanges();

            accountRelationship.AccountRelationshipId = entity.AccountRelationshipId;
        }

        public void Delete(int accountRelationshipId)
        {
            Entities.AccountRelationship entity =
                m_dbContext.AccountRelationships.SingleOrDefault(ar => ar.AccountRelationshipId == accountRelationshipId);

            if (entity == null)
                throw new ArgumentException(
                    $"No AccountRelationship exists with AccountRelationshipId {accountRelationshipId}",
                    nameof(accountRelationshipId)
                );

            m_dbContext.AccountRelationships.Remove(entity);
            m_dbContext.SaveChanges();
        }

        public AccountRelationship Get(int accountRelationshipId)
        {
            Entities.AccountRelationship entity =
                m_dbContext.AccountRelationships
                    .Include(ar => ar.SourceAccount)
                    .Include(ar => ar.DestinationAccount)
                    .SingleOrDefault(ar => ar.AccountRelationshipId == accountRelationshipId);

            if (entity == null)
                throw new ArgumentException(
                    $"No AccountRelationship exists with AccountRelationshipId {accountRelationshipId}",
                    nameof(accountRelationshipId)
                );

            return FromEntity(entity);
        }

        public IEnumerable<AccountRelationship> GetAll()
        {
            return m_dbContext.AccountRelationships
                .Include(ar => ar.SourceAccount)
                .Include(ar => ar.DestinationAccount)
                .Select(FromEntity)
                .ToList();
        }

        public void Update(AccountRelationship accountRelationship)
        {
            Entities.AccountRelationship entity = m_dbContext.AccountRelationships
                .Single(ar => ar.AccountRelationshipId == accountRelationship.AccountRelationshipId);

            entity.SourceAccountId = accountRelationship.SourceAccount.AccountId;
            entity.DestinationAccountId = accountRelationship.DestinationAccount.AccountId;
            entity.Type = accountRelationship.Type;

            m_dbContext.SaveChanges();
        }

        private AccountRelationship FromEntity(Entities.AccountRelationship entity)
        {
            return new AccountRelationship
            {
                AccountRelationshipId = entity.AccountRelationshipId,
                SourceAccount = FromAccountEntityToAccountLink(entity.SourceAccount),
                DestinationAccount = FromAccountEntityToAccountLink(entity.DestinationAccount),
                Type = entity.Type
            };
        }

        // TODO: duplicated code
        private static AccountLink FromAccountEntityToAccountLink(
            Entities.Account accountEntity)
        {
            return new AccountLink
            {
                AccountId = accountEntity.AccountId,
                Name = accountEntity.Name,
                Type = accountEntity.Type,
                SubType = accountEntity.SubType
            };
        }
    }
}
