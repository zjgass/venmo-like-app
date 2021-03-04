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
        public void Happypath()
        {
            // Arrange
            TransferSqlDAO dao = new TransferSqlDAO(connectionString);
            FromClientTransfer updatedTransfer = new FromClientTransfer()
            {
                TransferId = TestTransfer.TransferId,
                TransferType = "send",
                TransferStatus = "approved" // This is the part that gets updated.
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
            FromClientTransfer updatedTransfer = new FromClientTransfer()
            {
                TransferId = TestTransfer.TransferId,
                TransferType = "request",
                TransferStatus = "approved",
                Amount = 1000.00M
            };

            // Act
            bool updateComplete = dao.UpdateTransfer(updatedTransfer);

            // Assert
            Assert.IsTrue(updateComplete);
        }
    }
}
