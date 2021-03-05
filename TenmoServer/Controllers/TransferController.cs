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
            int userId = 0;
            try
            {
                userId = Int32.Parse(User.FindFirst("sub").Value);
            }
            catch (Exception)
            {
                return BadRequest();
            }

            List<Transfer> transfers = dao.GetAllTransfers(userId, true);

            if (transfers != null)
            {
                return Ok(transfers);
            }

            return BadRequest();
        }
        [HttpGet("pending")]
        public ActionResult<List<Transfer>> GetAllPendingTransfers()
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

            List<Transfer> transfers = dao.GetAllTransfers(userId, false);

            if (transfers != null)
            {
                return Ok(transfers);
            }

            return BadRequest();
        }
        [HttpGet("{id}", Name = "GetTransfer")]
        public ActionResult<Transfer> GetTransfer(int id)
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

            Transfer transfer = dao.GetTransfer(userId, id);

            if (transfer != null)
            {
                return Ok(transfer);
            }

            return NotFound();
        }
        [HttpPost]
        public ActionResult<Transfer> SendTransfer(Transfer transfer)
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

            if (userId == transfer.UserFromId)
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
            int userId = 0;
            try
            {
                userId = Int32.Parse(User.FindFirst("sub").Value);
            }
            catch (Exception)
            {
                return BadRequest();
            }

            if (userId == transfer.UserToId)
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
            int userId = 0;
            try
            {
                userId = Int32.Parse(User.FindFirst("sub").Value);
            }
            catch (Exception)
            {
                return BadRequest();
            }

            if (transfer.TransferStatus != "pending" && transfer.UserFromId == userId)
            {
                bool updateComplete = dao.UpdateTransfer(transfer);
                return CreatedAtRoute("GetTransfer", new { id = transfer.TransferId }, transfer);
            }

            return BadRequest();
        }
    }
}
