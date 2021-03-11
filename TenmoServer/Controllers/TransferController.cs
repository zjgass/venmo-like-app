using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using TenmoServer.DAO;
using TenmoServer.Models;

namespace TenmoServer.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    [Authorize]
    public class TransferController : ControllerBase
    {
        private readonly ITransferDAO transferDao;
        private readonly IAccountDAO accountDao;

        public TransferController(ITransferDAO _transferDao, IAccountDAO _accountDao)
        {
            transferDao = _transferDao;
            accountDao = _accountDao;
        }

        [HttpGet]
        public ActionResult<List<Transfer>> GetAllCompletedTransfers()
        {
            int userId = VerifyUser();

            List<Transfer> transfers = transferDao.GetAllTransfers(userId, true);

            if (transfers != null)
            {
                return Ok(transfers);
            }

            return BadRequest();
        }
        [HttpGet("pending")]
        public ActionResult<List<Transfer>> GetAllPendingTransfers()
        {
            int userId = VerifyUser();

            List<Transfer> transfers = transferDao.GetAllTransfers(userId, false);

            if (transfers != null && transfers.Count > 0)
            {
                return Ok(transfers);
            }
            else if (transfers.Count == 0)
            {
                return Ok("No pending transfers.");
            }

            return BadRequest();
        }
        [HttpGet("{id}", Name = "GetTransfer")]
        public ActionResult<Transfer> GetTransfer(int id)
        {
            int userId = VerifyUser();

            Transfer transfer = transferDao.GetTransfer(userId, id);

            if (transfer != null)
            {
                return Ok(transfer);
            }

            return NotFound();
        }
        [HttpPost]
        public ActionResult<Transfer> SendTransfer(Transfer transfer)
        {
            int userId = VerifyUser();

            if (userId == transfer.UserFromId && userId != transfer.UserToId)
            {
                try
                {
                    VerifyFunds(transfer);
                    transfer.TransferType = "Send";
                    transfer.TransferStatus = "Approved";

                    transfer = transferDao.NewTransfer(transfer);
                }
                catch (SqlException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
                
                return CreatedAtRoute("GetTransfer", new { id = transfer.TransferId }, transfer);
            }

            return BadRequest();
        }
        [HttpPost("request")]
        public ActionResult<Transfer> RequestTransfer(Transfer transfer)
        {
            int userId = VerifyUser();

            if (userId == transfer.UserToId)
            {
                transfer.TransferType = "Request";
                transfer.TransferStatus = "Pending";
                transfer = transferDao.NewTransfer(transfer);

                return CreatedAtRoute("GetTransfer", new { id = transfer.TransferId }, transfer);
            }

            return BadRequest();
        }
        [HttpPut("{id}")]
        public ActionResult<Transfer> UpdateTransfer(Transfer transfer)
        {
            int userId = VerifyUser();

            if (transfer.TransferStatus.ToLower().Trim() != "pending" && transfer.UserFromId == userId)
            {
                if (transfer.TransferStatus.ToLower().Trim().Equals("approved"))
                {
                    try
                    {
                        VerifyFunds(transfer);

                        transfer = transferDao.UpdateTransfer(transfer);
                    }
                    catch (SqlException)
                    {
                        throw;
                    }
                    catch (Exception e)
                    {
                        return BadRequest(e.Message);
                    }

                    return CreatedAtRoute("GetTransfer", new { id = transfer.TransferId }, transfer);
                }
                else if (transfer.TransferStatus.ToLower().Trim().Equals("rejected"))
                {
                    transfer = transferDao.UpdateTransfer(transfer);
                }

                return CreatedAtRoute("GetTransfer", new { id = transfer.TransferId }, transfer);
            }
            else
            {
                return BadRequest("Invalid transfer.");
            }

            return BadRequest();
        }

        private Transfer CreateTransfer(Transfer transfer)
        {
            transfer = transferDao.NewTransfer(transfer);
            transfer = transferDao.GetTransfer(transfer.UserFromId, transfer.TransferId);

            return transfer;
        }
        private int VerifyUser()
        {
            int userId = 0;
            try
            {
                userId = Int32.Parse(User.FindFirst("sub").Value);
            }
            catch (Exception)
            {
                throw new Exception("Invalid user id.");
            }

            return userId;
        }

        private bool VerifyFunds(Transfer transfer)
        {
            decimal balance = accountDao.GetAccount(transfer.UserFromId).Balance;

            if (transfer.Amount > balance)
            {
                throw new Exception("Insufficient funds.");
            }

            return true;
        }
    }
}
