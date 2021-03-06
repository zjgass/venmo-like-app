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
    public class DepositTests : DAOTestClass
    {
        [TestMethod]
        public void HappyPath()
        {
            // Arrange
            AccountSqlDAO dao = new AccountSqlDAO(connectionString);
            Account accountTesting = new Account();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sqlText = "select account_id from accounts where user_id = @userId;";
                    SqlCommand cmd = new SqlCommand(sqlText, conn);
                    cmd.Parameters.AddWithValue("@userId", TestUser2.UserId);
                    accountTesting.AccountId = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception)
            {
            }

            // Act and Assert
            Assert.IsTrue(dao.Deposit(accountTesting, 50.00M));
        }
    }
}
