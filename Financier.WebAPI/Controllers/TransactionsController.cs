using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Financier.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Financier.WebAPI.Controllers
{
    public class TempApiTransaction
    {
        public string DebitAccountName { get; set; }
        public string CreditAccountName { get; set; }
        public DateTime At { get; set; }
        public decimal Amount { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private FinancierDbContext m_dbContext;

        public TransactionsController(FinancierDbContext dbContext)
        {
            m_dbContext = dbContext;
        }

        // GET api/transactions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TempApiTransaction>>> Get()
        {
            List<Transaction> transactions = await m_dbContext.Transactions
                .Include(t => t.CreditAccount)
                .Include(t => t.DebitAccount)
                .OrderByDescending(t => t.At)
                .Take(100)
                .ToListAsync();

            List< TempApiTransaction> apiResponse = transactions.Select(t => new TempApiTransaction
            {
                DebitAccountName = t.DebitAccount.Name,
                CreditAccountName = t.CreditAccount.Name,
                At = t.At,
                Amount = t.Amount
            }).ToList();

            return apiResponse;
        }
    }
}