using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.DAO;
using TenmoServer.Models;

namespace TenmoServer.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAccountDAO accountDAO;

        public AccountController(IAccountDAO _accountDAO)
        {
            accountDAO = _accountDAO;
        }

        [HttpGet]
        public ActionResult<Account> GetCurrentUserAccount()
        {
            int userId = 0;
            try
            {
                userId = Int32.Parse(User.FindFirst("sub").Value);
            }
            catch (Exception)
            {
                return BadRequest();
            }

            Account account = accountDAO.GetAccount(userId);

            if (account != null)
            {
                return Ok(account);
            }

            return NotFound();
        }
    }
}
