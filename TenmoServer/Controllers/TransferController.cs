﻿using Microsoft.AspNetCore.Authorization;
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
                return BadRequest();
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
                transfer = transferDao.NewTransfer(transfer);

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
                bool updateComplete = transferDao.UpdateTransfer(transfer);
                return CreatedAtRoute("GetTransfer", new { id = transfer.TransferId }, transfer);
            }

            return BadRequest();
        }

        private Transfer ExecuteTransfer(Transfer transfer)
        {
            decimal balance = accountDAO.GetAccount(transfer.UserFromId).Balance;

            if (transfer.Amount < balance)
            {
                throw new Exception("Insufficient funds.");
            }

            using(TransactionScope transaction = new TransactionScope())
            {
                // Get the sum of the initial balances to do a check.
                decimal initSum = accountDAO.GetAccount(transfer.UserFromId).Balance
                                    + accountDAO.GetAccount(transfer.UserToId).Balance;

                // Deposit.
                Account toAccount = accountDAO.GetAccount(transfer.UserToId);
                accountDAO.Deposit(toAccount, transfer.Amount);

                // Withdraw.
                Account fromAccount = accountDAO.GetAccount(transfer.UserFromId);
                accountDAO.Withdraw(fromAccount, transfer.Amount);

                // Get the sum of the final balance to do a check.
                decimal finalSum = accountDAO.GetAccount(transfer.UserFromId).Balance
                                    + accountDAO.GetAccount(transfer.UserToId).Balance;

                // Verify the sum of balances are equal.
                if (initSum == finalSum)
                {
                    transaction.Complete();
                }
                else
                {
                    transaction.Dispose();
                }

                return transfer;
            }
        }
    }
}
