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
        public void BadUserId()
        {
            // Arrange
            TransferSqlDAO dao = new TransferSqlDAO(connectionString);

            // Act
            IList<Transfer> transferList = dao.GetAllTransfers(Int32.MaxValue, true);

            // Assert
            Assert.IsTrue(transferList.Count == 0);
        }
    }
}
