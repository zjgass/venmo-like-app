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
    public class GetTransferTests : DAOTestClass
    {
        [TestMethod]
        public void HappyPath()
        {
            // Arrange
            TransferSqlDAO dao = new TransferSqlDAO(connectionString);

            // Act
            Transfer transfer = dao.GetTransfer(TestUser1.UserId, TestTransfer.TransferId);

            // Assert
            Assert.IsTrue(transfer.TransferStatus.ToLower().Equals(TestTransfer.TransferStatus.ToLower()));
        }

        [TestMethod]
        public void BigBadTransferNumber()
        {
            // Arrange
            TransferSqlDAO dao = new TransferSqlDAO(connectionString);

            // Act
            Transfer transfer = dao.GetTransfer(TestUser1.UserId, Int32.MaxValue);

            // Assert
            Assert.IsNull(transfer.TransferStatus);
        }

        [DataTestMethod]
        [DataRow(-1)]
        [DataRow(-5)]
        [DataRow(-10)]
        [DataRow(-100)]
        [DataRow(Int32.MinValue)]
        public void NegativeTransferNumber(int id)
        {
            // Arrange
            TransferSqlDAO dao = new TransferSqlDAO(connectionString);

            // Act
            Transfer transfer = dao.GetTransfer(TestUser1.UserId, id);

            // Assert
            Assert.IsNull(transfer.TransferStatus);
        }

        [DataTestMethod]
        [DataRow(-1)]
        [DataRow(-5)]
        [DataRow(-10)]
        [DataRow(-100)]
        [DataRow(Int32.MinValue)]
        public void NegativeUserId(int id)
        {
            // Arrange
            TransferSqlDAO dao = new TransferSqlDAO(connectionString);

            // Act
            Transfer transfer = dao.GetTransfer(id, TestTransfer.TransferId);

            // Assert
            Assert.IsNull(transfer.TransferStatus);
        }

        [TestMethod]
        public void BigBadUserNumber()
        {
            // Arrange
            TransferSqlDAO dao = new TransferSqlDAO(connectionString);

            // Act
            Transfer transfer = dao.GetTransfer(Int32.MaxValue, TestTransfer.TransferId);

            // Assert
            Assert.IsNull(transfer.TransferStatus);
        }
    }
}
