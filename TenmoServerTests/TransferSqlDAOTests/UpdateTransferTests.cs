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
            bool updateCompleted = dao.UpdateTransfer(updatedTransfer);

            // Assert
            Assert.IsFalse(TestTransfer.TransferStatus.ToLower().Equals(updatedTransfer.TransferStatus.ToLower()));
        }

        [TestMethod]
        public void ChangeAmount()
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
            bool updateComplete = dao.UpdateTransfer(updatedTransfer);
            decimal unchangedBalance = 0;

            try
            {
                using(SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sqlText = "select amount from transfers where transfer_id = @transferId;";
                    SqlCommand cmd = new SqlCommand(sqlText, conn);
                    cmd.Parameters.AddWithValue("@transferId", TestTransfer.TransferId);

                    // Why is this not working?
                    //unchangedBalance = Convert.ToDecimal(cmd.ExecuteScalar());

                    SqlDataReader reader = cmd.ExecuteReader();
                    unchangedBalance = Convert.ToDecimal(reader["amount"]);
                }
            }
            catch (Exception)
            {
            }

            // Assert
            Assert.IsTrue(TestTransfer.Amount == unchangedBalance);
        }
    }
}
