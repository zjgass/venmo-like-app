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
    public class NewTransferTests : DAOTestClass
    {
        [TestMethod]
        public void HappyPath()
        {
            // Arrange
            TransferSqlDAO dao = new TransferSqlDAO(connectionString);
            Transfer newTransfer = new Transfer()
            {
                TransferType = "send",
                TransferStatus = "approved",
                UserFromId = TestUser1.UserId,
                UserToId = TestUser2.UserId,
                Amount = 50.00M
            };

            int startingtransfers = GetNumberOfTransfers(TestUser1.UserId);

            // Act
            dao.NewTransfer(newTransfer);

            int endingtransfers = GetNumberOfTransfers(TestUser1.UserId);

            // Assert
            Assert.IsTrue(endingtransfers > startingtransfers);
        }

        [TestMethod]
        [ExpectedException(typeof(SqlException))]
        public void NoAmmountTransfer()
        {
            // Arrange
            TransferSqlDAO dao = new TransferSqlDAO(connectionString);
            Transfer newTransfer = new Transfer()
            {
                TransferType = "send",
                TransferStatus = "approved",
                UserFromId = TestUser1.UserId,
                UserToId = TestUser2.UserId,
                //Amount = 50.00M
            };

            int startingtransfers = GetNumberOfTransfers(TestUser1.UserId);

            // Act
            dao.NewTransfer(newTransfer);

            int endingtransfers = GetNumberOfTransfers(TestUser1.UserId);

            // Assert
            Assert.IsTrue(endingtransfers > startingtransfers);
        }

        [TestMethod]
        [ExpectedException(typeof(SqlException))]
        public void NegativeAmmountTransfer()
        {
            // Arrange
            TransferSqlDAO dao = new TransferSqlDAO(connectionString);
            Transfer newTransfer = new Transfer()
            {
                TransferType = "send",
                TransferStatus = "approved",
                UserFromId = TestUser1.UserId,
                UserToId = TestUser2.UserId,
                Amount = -50.00M
            };

            int startingtransfers = GetNumberOfTransfers(TestUser1.UserId);

            // Act
            dao.NewTransfer(newTransfer);

            int endingtransfers = GetNumberOfTransfers(TestUser1.UserId);

            // Assert
            Assert.IsTrue(endingtransfers > startingtransfers);
        }

        [TestMethod]
        [ExpectedException(typeof(SqlException))]
        public void TooLargeAmmountTransfer()
        {
            // Arrange
            TransferSqlDAO dao = new TransferSqlDAO(connectionString);
            Transfer newTransfer = new Transfer()
            {
                TransferType = "send",
                TransferStatus = "approved",
                UserFromId = TestUser1.UserId,
                UserToId = TestUser2.UserId,
                Amount = Decimal.MaxValue
            };

            int startingtransfers = GetNumberOfTransfers(TestUser1.UserId);

            // Act
            dao.NewTransfer(newTransfer);

            int endingtransfers = GetNumberOfTransfers(TestUser1.UserId);

            // Assert
            Assert.IsTrue(endingtransfers > startingtransfers);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void BadUserTransfer()
        {
            // Arrange
            TransferSqlDAO dao = new TransferSqlDAO(connectionString);
            Transfer newTransfer = new Transfer()
            {
                TransferType = "send",
                TransferStatus = "approved",
                //UserFromId = TestUser1.UserId,
                UserToId = TestUser2.UserId,
                Amount = 50.00M
            };

            int startingtransfers = GetNumberOfTransfers(TestUser1.UserId);

            // Act
            dao.NewTransfer(newTransfer);

            int endingtransfers = GetNumberOfTransfers(TestUser1.UserId);

            // Assert
            Assert.IsTrue(endingtransfers > startingtransfers);
        }

        public void NegativeUserTransfer()
        {
            // Arrange
            TransferSqlDAO dao = new TransferSqlDAO(connectionString);
            Transfer newTransfer = new Transfer()
            {
                TransferType = "send",
                TransferStatus = "approved",
                UserFromId = -1,
                UserToId = TestUser2.UserId,
                Amount = 50.00M
            };

            int startingtransfers = GetNumberOfTransfers(TestUser1.UserId);

            // Act
            dao.NewTransfer(newTransfer);

            int endingtransfers = GetNumberOfTransfers(TestUser1.UserId);

            // Assert
            Assert.IsTrue(endingtransfers > startingtransfers);
        }

        public int GetNumberOfTransfers(int userId)
        {
            int totalTransfers = 0;

            try
            {
                using(SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand($"select count(*) from transfers " +
                        "where (account_to = (select account_id from accounts where user_id = @userId) " +
                        "or account_from = (select account_id from accounts where user_id = @userId)); " +
                        "select scope_identity();", conn);
                    cmd.Parameters.AddWithValue("@userId", userId);

                    totalTransfers = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return totalTransfers;
        }
    }
}
