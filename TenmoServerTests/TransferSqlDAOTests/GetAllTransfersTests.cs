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
    public class GetAllTransfersTests : DAOTestClass
    {
        [TestMethod]
        public void HappyPath()
        {
            // Arrange
            TransferSqlDAO dao = new TransferSqlDAO(connectionString);

            // Act
            IList<Transfer> transferList = dao.GetAllTransfers(TestUser1.UserId, true);

            // Assert
            Assert.IsTrue(transferList.Count > 0);
        }

        [TestMethod]
        public void PendingHappyPath()
        {
            // Arrange
            TransferSqlDAO dao = new TransferSqlDAO(connectionString);

            // Act
            IList<Transfer> transferList = dao.GetAllTransfers(TestUser1.UserId, false);

            // Assert
            Assert.IsTrue(transferList.Count > 0);
        }

        [TestMethod]
        public void BigUserId()
        {
            // Arrange
            TransferSqlDAO dao = new TransferSqlDAO(connectionString);

            // Act
            IList<Transfer> transferList = dao.GetAllTransfers(Int32.MaxValue, true);

            // Assert
            Assert.IsTrue(transferList.Count == 0);
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
            IList<Transfer> transferList = dao.GetAllTransfers(id, true);

            // Assert
            Assert.IsTrue(transferList.Count == 0);
        }
    }
}
