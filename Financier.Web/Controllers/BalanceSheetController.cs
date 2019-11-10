using System;
using Financier.Services;
using Microsoft.AspNetCore.Mvc;

namespace Financier.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BalanceSheetController : ControllerBase
    {
        private IBalanceSheetService m_balanceSheetService;

        public BalanceSheetController(IBalanceSheetService balanceSheetService)
        {
            m_balanceSheetService = balanceSheetService;
        }

        [HttpGet]
        public ActionResult<BalanceSheet> Get()
        {
            return m_balanceSheetService.Generate(DateTime.Now);
        }
    }
}