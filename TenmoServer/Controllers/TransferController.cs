using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
        private readonly IAccountDAO accountDAO;

        public TransferController(ITransferDAO _transferDao, IAccountDAO _accountDao)
        {
            transferDao = _transferDao;
            accountDAO = _accountDao;
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
                return BadRequest("Invalid user id.");
            }

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
            int userId = 0;
            try
            {
                userId = Int32.Parse(User.FindFirst("sub").Value);
            }
            catch (Exception)
            {
                return BadRequest();
            }

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
            int userId = 0;
            try
            {
                userId = Int32.Parse(User.FindFirst("sub").Value);
            }
            catch (Exception)
            {
                return BadRequest();
            }

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
            bool success = false;
            int userId = 0;
            try
            {
                userId = Int32.Parse(User.FindFirst("sub").Value);
            }
            catch (Exception)
            {
                return BadRequest();
            }

            if (userId == transfer.UserFromId && userId != transfer.UserToId)
            {
                transfer.TransferType = "Send";
                transfer.TransferStatus = "Approved";

                try
                {
                    success = ExecuteTransfer(transfer);
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }

                if (success)
                {
                    transfer = CreateTransfer(transfer);
                }
                
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
                transfer = transferDao.NewTransfer(transfer);
                transfer = transferDao.GetTransfer(transfer.UserFromId, transfer.TransferId);

                return CreatedAtRoute("GetTransfer", new { id = transfer.TransferId }, transfer);
            }

            return BadRequest();
        }
        [HttpPut("{id}")]
        public ActionResult<Transfer> UpdateTransfer(Transfer transfer)
        {
            bool success = false;
            int userId = 0;
            try
            {
                userId = Int32.Parse(User.FindFirst("sub").Value);
            }
            catch (Exception)
            {
                return BadRequest("Invalid user id.");
            }

            if (transfer.TransferStatus.ToLower().Trim() != "pending" && transfer.UserFromId == userId)
            {
                if (transfer.TransferStatus.ToLower().Trim().Equals("approved"))
                {
                    try
                    {
                        success = ExecuteTransfer(transfer);
                    }
                    catch (Exception e)
                    {
                        return BadRequest(e.Message);
                    }

                    if (success)
                    {
                        transfer = transferDao.UpdateTransfer(transfer);
                        transfer = transferDao.GetTransfer(transfer.UserFromId, transfer.TransferId);
                    }
                    
                }
                else if (transfer.TransferStatus.ToLower().Trim().Equals("rejected"))
                {
                    success = true;
                    transfer = transferDao.UpdateTransfer(transfer);
                    transfer = transferDao.GetTransfer(transfer.UserFromId, transfer.TransferId);
                }

                if (success)
                {
                    return CreatedAtRoute("GetTransfer", new { id = transfer.TransferId }, transfer);
                }
            }
            else
            {
                return BadRequest("Invalid transfer.");
            }

            return BadRequest();
        }

        private bool ExecuteTransfer(Transfer transfer)
        {
            bool executeSuccessful = false;
            bool depositSuccess = false;
            bool withdrawSuccess = false;
            decimal balance = accountDAO.GetAccount(transfer.UserFromId).Balance;

            if (transfer.Amount > balance)
            {
                throw new Exception("Insufficient funds.");
            }

            try
            {
                using (TransactionScope transaction = new TransactionScope())
                {
                    // Get the sum of the initial balances to do a check.
                    decimal initSum = accountDAO.GetAccount(transfer.UserFromId).Balance
                                    + accountDAO.GetAccount(transfer.UserToId).Balance;

                    // Deposit.
                    Account toAccount = accountDAO.GetAccount(transfer.UserToId);
                    depositSuccess = accountDAO.Deposit(toAccount, transfer.Amount);

                    // Withdraw.
                    Account fromAccount = accountDAO.GetAccount(transfer.UserFromId);
                    withdrawSuccess = accountDAO.Withdraw(fromAccount, transfer.Amount);

                    // Get the sum of the final balance to do a check.
                    decimal finalSum = accountDAO.GetAccount(transfer.UserFromId).Balance
                                     + accountDAO.GetAccount(transfer.UserToId).Balance;

                    // Verify the sum of balances are equal.
                    if (initSum == finalSum && depositSuccess && withdrawSuccess)
                    {
                        transaction.Complete();
                        executeSuccessful = true;
                    }
                    else
                    {
                        transaction.Dispose();
                        throw new Exception("Sorry error, please try again.");
                    }

                    return executeSuccessful;
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private Transfer CreateTransfer(Transfer transfer)
        {
            transfer = transferDao.NewTransfer(transfer);
            transfer = transferDao.GetTransfer(transfer.UserFromId, transfer.TransferId);

            return transfer;
        }
    }
}
