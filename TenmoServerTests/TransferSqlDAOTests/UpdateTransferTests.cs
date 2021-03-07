using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using TenmoServer.Models;
using TenmoServer.DAO;
using System.Data.SqlClient;

namespace TenmoServerTests.TransferSqlDAOTests
{
    [TestClass]
    public class UpdateTransferTests : DAOTestClass
    {
        [TestMethod]
        public void HappyPath()
        {
            // Arrange
            TransferSqlDAO dao = new TransferSqlDAO(connectionString);
            Transfer updatedTransfer = new Transfer()
            {
                TransferId = TestTransfer.TransferId,
                TransferType = "request",
                TransferStatus = "approved", // This is the part that gets updated.
                UserFromId = TestTransfer.UserFromId,
                UserToId = TestTransfer.UserToId,
                Amount = 50.00M
            };

            // Act
            updatedTransfer = dao.UpdateTransfer(updatedTransfer);

            // Assert
            Assert.IsFalse(TestTransfer.TransferStatus.ToLower().Equals(updatedTransfer.TransferStatus.ToLower()));
        }

        [TestMethod]
        public void CannotChangeAmount()
        {
            // Arrange
            TransferSqlDAO dao = new TransferSqlDAO(connectionString);
            Transfer updatedTransfer = new Transfer()
            {
                TransferId = TestTransfer.TransferId,
                TransferType = "request",
                TransferStatus = "approved",
                UserFromId = TestTransfer.UserFromId,
                UserToId = TestTransfer.UserToId,
                Amount = 1000.00M
            };

            // Act
            updatedTransfer = dao.UpdateTransfer(updatedTransfer);
            decimal unchangedBalance = 0;

            try
            {
                using(SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string sqlText = "select amount from transfers where transfer_id = @transferId;";
                    SqlCommand cmd = new SqlCommand(sqlText, conn);
                    cmd.Parameters.AddWithValue("@transferId", TestTransfer.TransferId);

                    unchangedBalance = Convert.ToDecimal(cmd.ExecuteScalar());
                }
            }
            catch (Exception e)
            {
            }

            // Assert
            Assert.IsTrue(TestTransfer.Amount == unchangedBalance);
        }

        [TestMethod]
        public void CannotChangeReceiver()
        {
            // Arrange
            TransferSqlDAO dao = new TransferSqlDAO(connectionString);
            Transfer updatedTransfer = new Transfer()
            {
                TransferId = TestTransfer.TransferId,
                TransferType = "request",
                TransferStatus = "approved",
                UserFromId = TestTransfer.UserFromId,
                UserToId = TestTransfer.UserToId + 1,
                Amount = 1000.00M
            };

            // Act
            updatedTransfer = dao.UpdateTransfer(updatedTransfer);
            int unchangedUser = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string sqlText = "select user_id from transfers " +
                        "join accounts on accounts.account_id = transfers.account_to " +
                        "join users on users.user_id = accounts.user_id " +
                        "where transfer_id = @transferId;";
                    SqlCommand cmd = new SqlCommand(sqlText, conn);
                    cmd.Parameters.AddWithValue("@transferId", TestTransfer.TransferId);

                    unchangedUser = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception e)
            {
            }

            // Assert
            Assert.IsTrue(TestTransfer.UserToId == unchangedUser);
        }
    }
}
