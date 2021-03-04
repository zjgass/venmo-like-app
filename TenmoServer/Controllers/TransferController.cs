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

            if (transfers != null)
            {
                return Ok(transfers);
            }

            return BadRequest();
        }
        [HttpGet("pending")]
        public ActionResult<List<Transfer>> GetAllPendingTransfers()
        {
            List<Transfer> transfers = dao.GetAllTransfers(User.Identity.Name, false);

            if (transfers != null)
            {
                return Ok(transfers);
            }

            return BadRequest();
        }
        [HttpGet("{id}", Name = "GetTransfer")]
        public ActionResult<Transfer> GetTransfer(int id)
        {
            Transfer transfer = dao.GetTransfer(User.Identity.Name, id);

            if (transfer != null)
            {
                return Ok(transfer);
            }

            return NotFound();
        }
        [HttpPost]
        public ActionResult<Transfer> SendTransfer(Transfer transfer)
        {
            if (User.Identity.Name == transfer.UserFrom)
            {
                transfer.TransferType = "Send";
                transfer.TransferStatus = "Approved";
                transfer = dao.NewTransfer(transfer);

                return CreatedAtRoute("GetTransfer", new { id = transfer.TransferId }, transfer);
            }

            return BadRequest();
        }
        [HttpPost("request")]
        public ActionResult<Transfer> RequestTransfer(Transfer transfer)
        {
            if (User.Identity.Name == transfer.UserTo)
            {
                transfer.TransferType = "Request";
                transfer.TransferStatus = "Pending";
                transfer = dao.NewTransfer(transfer);

                return CreatedAtRoute("GetTransfer", new { id = transfer.TransferId }, transfer);
            }

            return BadRequest();
        }
        [HttpPut("{id}")]
        public ActionResult<Transfer> UpdateTransfer(Transfer transfer)
        {
            if (transfer.TransferStatus != "pending" && transfer.UserFrom == User.Identity.Name)
            {
                transfer = dao.UpdateTransfer(transfer);
                return CreatedAtRoute("GetTransfer", new { id = transfer.TransferId }, transfer);
            }

            return BadRequest();
        }
    }
}
