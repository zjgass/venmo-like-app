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
    public class WithdrawTests : DAOTestClass
    {
        [TestMethod]
        public void HappyPath()
        {
            // Arrange
            AccountSqlDAO dao = new AccountSqlDAO(connectionString);
            Account account = new Account();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string sqlText = "select account_id, user_id, balance from accounts where user_id = @userId;";
                    SqlCommand cmd = new SqlCommand(sqlText, conn);
                    cmd.Parameters.AddWithValue("@userId", TestUser2.UserId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if(reader.HasRows && reader.Read())
                    {
                        account.AccountId = Convert.ToInt32(reader["account_id"]);
                        account.UserId = Convert.ToInt32(reader["user_id"]);
                        account.Balance = Convert.ToDecimal(reader["balance"]);
                    }
                }
            }
            catch (Exception e)
            {
            }

            // Act and Assert
            Assert.IsTrue(dao.Withdraw(account, 50.00M));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void NegativeAmount()
        {
            // Arrange
            AccountSqlDAO dao = new AccountSqlDAO(connectionString);
            Account account = new Account();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string sqlText = "select account_id, user_id, balance from accounts where user_id = @userId;";
                    SqlCommand cmd = new SqlCommand(sqlText, conn);
                    cmd.Parameters.AddWithValue("@userId", TestUser2.UserId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows && reader.Read())
                    {
                        account.AccountId = Convert.ToInt32(reader["account_id"]);
                        account.UserId = Convert.ToInt32(reader["user_id"]);
                        account.Balance = Convert.ToDecimal(reader["balance"]);
                    }
                }
            }
            catch (Exception e)
            {
            }

            // Act and Assert
            Assert.IsTrue(dao.Withdraw(account, -50.00M));
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ZeroAmount()
        {
            // Arrange
            AccountSqlDAO dao = new AccountSqlDAO(connectionString);
            Account account = new Account();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string sqlText = "select account_id, user_id, balance from accounts where user_id = @userId;";
                    SqlCommand cmd = new SqlCommand(sqlText, conn);
                    cmd.Parameters.AddWithValue("@userId", TestUser2.UserId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows && reader.Read())
                    {
                        account.AccountId = Convert.ToInt32(reader["account_id"]);
                        account.UserId = Convert.ToInt32(reader["user_id"]);
                        account.Balance = Convert.ToDecimal(reader["balance"]);
                    }
                }
            }
            catch (Exception e)
            {
            }

            // Act and Assert
            Assert.IsTrue(dao.Withdraw(account, 0));
        }
    }
}
