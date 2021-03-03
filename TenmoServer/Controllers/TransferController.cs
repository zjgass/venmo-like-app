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
    public class TransferController : ControllerBase
    {
        private readonly ITransferDAO dao;

        public TransferController(ITransferDAO transferDao)
        {
            dao = transferDao;
        }

        [HttpGet]
        public ActionResult<List<Transfer>> GetAllCompletedTransfers()
        {
            List<Transfer> transfer = dao.GetAllTransfers(User.Identity.UserId, true);

            return Ok(transfer);
        }
    }
}
