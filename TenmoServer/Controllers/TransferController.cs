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
            List<Transfer> transfers = dao.GetAllTransfers(User.Identity.Name, true);

            return Ok(transfers);
        }
        [HttpGet("pending")]
        public ActionResult<List<Transfer>> GetAllPendingTransfers()
        {
            List<Transfer> transfers = dao.GetAllTransfers(User.Identity.Name, false);

            return Ok(transfers);
        }
        [HttpGet("{id}", Name = "GetTransfer")]
        public ActionResult<Transfer> GetTransfer(int id)
        {
            Transfer transfer = dao.GetTransfer(User.Identity.Name, id);

            return Ok(transfer);
        }
        [HttpPost]
        public ActionResult<Transfer> SendTransfer(Transfer transfer)
        {
            //dao.SendTransfer();

            return Ok(transfer);
        }
    }
}
