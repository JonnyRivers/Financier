using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Financier.Web.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Financier.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly ILogger<ExpensesController> m_logger;
        private readonly Entities.FinancierDbContext m_dbContext;

        public AccountsController(ILoggerFactory loggerFactory, Entities.FinancierDbContext dbContext)
        {
            m_logger = loggerFactory.CreateLogger<ExpensesController>();
            m_dbContext = dbContext;
        }

        // GET api/accounts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccountSummary>>> Get()
        {
            List<Entities.Account> accountEntities = await m_dbContext.Accounts.ToListAsync();
            List<AccountSummary> accountSummaries = accountEntities.Select(a => new AccountSummary { Id = a.AccountId, Name = a.Name }).ToList();

            return accountSummaries;
        }
    }
}