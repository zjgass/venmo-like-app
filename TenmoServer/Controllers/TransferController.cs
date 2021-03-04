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
        public ActionResult<List<ToClientTransfer>> GetAllCompletedTransfers()
        {
            List<ToClientTransfer> transfers = dao.GetAllTransfers(User.Identity.Name, true);

            if (transfers != null)
            {
                return Ok(transfers);
            }

            return BadRequest();
        }
        [HttpGet("pending")]
        public ActionResult<List<ToClientTransfer>> GetAllPendingTransfers()
        {
            List<ToClientTransfer> transfers = dao.GetAllTransfers(User.Identity.Name, false);

            if (transfers != null)
            {
                return Ok(transfers);
            }

            return BadRequest();
        }
        [HttpGet("{id}", Name = "GetTransfer")]
        public ActionResult<ToClientTransfer> GetTransfer(int id)
        {
            ToClientTransfer transfer = dao.GetTransfer(User.Identity.Name, id);

            if (transfer != null)
            {
                return Ok(transfer);
            }

            return NotFound();
        }
        [HttpPost]
        public ActionResult<ToClientTransfer> SendTransfer(FromClientTransfer transferIn)
        {
            if (User.Identity.Name == transferIn.Author)
            {
                transferIn.TransferType = "Send";
                transferIn.TransferStatus = "Approved";
                ToClientTransfer transferOut = dao.NewTransfer(transferIn);

                return CreatedAtRoute("GetTransfer", new { id = transferOut.TransferId }, transferOut);
            }

            return BadRequest();
        }
        [HttpPost("request")]
        public ActionResult<ToClientTransfer> RequestTransfer(FromClientTransfer transferIn)
        {
            if (User.Identity.Name == transferIn.Author)
            {
                transferIn.TransferType = "Request";
                transferIn.TransferStatus = "Pending";
                ToClientTransfer transferOut = dao.NewTransfer(transferIn);

                return CreatedAtRoute("GetTransfer", new { id = transferOut.TransferId }, transferOut);
            }

            return BadRequest();
        }
        [HttpPut("{id}")]
        public ActionResult<ToClientTransfer> UpdateTransfer(FromClientTransfer transfer)
        {
            if (transfer.TransferStatus != "pending" && transfer.Author == User.Identity.Name)
            {
                bool updateComplete = dao.UpdateTransfer(transfer);
                return CreatedAtRoute("GetTransfer", new { id = transfer.TransferId }, transfer);
            }

            return BadRequest();
        }
    }
}
