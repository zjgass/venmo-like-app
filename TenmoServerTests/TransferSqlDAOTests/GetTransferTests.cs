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
            ToClientTransfer transfer = dao.GetTransfer(TestUser1.Username, TestTransfer.TransferId);

            // Assert
            Assert.IsTrue(transfer.TransferStatus.ToLower().Equals(TestTransfer.TransferStatus.ToLower()));
        }

        [TestMethod]
        [ExpectedException(typeof(SqlException))]
        public void NullUserName()
        {
            // Arrange
            TransferSqlDAO dao = new TransferSqlDAO(connectionString);

            // Act
            ToClientTransfer transfer = dao.GetTransfer(null, TestTransfer.TransferId);

            // Assert
            Assert.IsTrue(transfer.TransferStatus.ToLower().Equals(TestTransfer.TransferStatus.ToLower()));
        }

        [TestMethod]
        public void BigBadTransferNumber()
        {
            // Arrange
            TransferSqlDAO dao = new TransferSqlDAO(connectionString);

            // Act
            ToClientTransfer transfer = dao.GetTransfer(TestUser1.Username, Int32.MaxValue);

            // Assert
            Assert.IsNull(transfer.TransferStatus);
        }

        [TestMethod]
        public void NegativeTransferNumber()
        {
            // Arrange
            TransferSqlDAO dao = new TransferSqlDAO(connectionString);

            // Act
            ToClientTransfer transfer = dao.GetTransfer(TestUser1.Username, -1);

            // Assert
            Assert.IsNull(transfer.TransferStatus);
        }
    }
}
