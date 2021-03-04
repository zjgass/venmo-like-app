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
            IList<ToClientTransfer> transferList = dao.GetAllTransfers(TestUser1.Username, true);

            // Assert
            Assert.IsTrue(transferList.Count > 0);
        }

        [TestMethod]
        public void PendingHappyPath()
        {
            // Arrange
            TransferSqlDAO dao = new TransferSqlDAO(connectionString);

            // Act
            IList<ToClientTransfer> transferList = dao.GetAllTransfers(TestUser1.Username, false);

            // Assert
            Assert.IsTrue(transferList.Count > 0);
        }

        [TestMethod]
        public void BadUserName()
        {
            // Arrange
            TransferSqlDAO dao = new TransferSqlDAO(connectionString);

            // Act
            IList<ToClientTransfer> transferList = dao.GetAllTransfers("bobIsAUserNameThatDoesNotExist", true);

            // Assert
            Assert.IsTrue(transferList.Count == 0);
        }

        [TestMethod]
        [ExpectedException(typeof(SqlException))]
        public void NullUserName()
        {
            // Arrange
            TransferSqlDAO dao = new TransferSqlDAO(connectionString);

            // Act
            IList<ToClientTransfer> transferList = dao.GetAllTransfers(null, true);

            // Assert
            Assert.IsTrue(transferList.Count == 0);
        }
    }
}
