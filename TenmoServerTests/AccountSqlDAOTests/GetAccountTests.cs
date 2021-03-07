using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using TenmoServer.Models;
using TenmoServer.DAO;
using System.Data.SqlClient;

namespace TenmoServerTests.AccountSqlDAOTests
{
    [TestClass]
    public class GetAccountTests : DAOTestClass
    {
        [TestMethod]
        public void HappyPath()
        {
            // Arrange
            AccountSqlDAO dao = new AccountSqlDAO(connectionString);

            // Act
            Account account = dao.GetAccount(TestUser1.UserId);

            // Assert
            Assert.IsTrue(account.Balance == 1000.00M);
        }

        [TestMethod]
        public void BigBadUserNumber()
        {
            // Arrange
            AccountSqlDAO dao = new AccountSqlDAO(connectionString);

            // Act
            Account account = dao.GetAccount(Int32.MaxValue);

            // Assert
            Assert.IsTrue(account.AccountId == 0 && account.UserId == 0 && account.Balance == 0);
        }

        [DataTestMethod]
        [DataRow(-1)]
        [DataRow(-5)]
        [DataRow(-10)]
        [DataRow(-100)]
        [DataRow(Int32.MinValue)]
        public void NegativeUserNumber(int id)
        {
            // Arrange
            AccountSqlDAO dao = new AccountSqlDAO(connectionString);

            // Act
            Account account = dao.GetAccount(id);

            // Assert
            Assert.IsTrue(account.AccountId == 0 && account.UserId == 0 && account.Balance == 0);
        }
    }
}
