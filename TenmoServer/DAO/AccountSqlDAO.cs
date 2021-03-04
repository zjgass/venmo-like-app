using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public class AccountSqlDAO : IAccountDAO
    {

        private readonly string connectionString;
        const decimal startingBalance = 1000;

        public AccountSqlDAO(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }
        public Account GetAccount(int userId)
        {


            Account accountInfo = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT account_id, user_id, ballance FROM accounts WHERE user_id = @userid", conn);
                    cmd.Parameters.AddWithValue("@userid", userId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows && reader.Read())
                    {
                        accountInfo = GetAccountFromReader(reader);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return accountInfo;
        }

        private Account GetAccountFromReader(SqlDataReader reader)
        {
            Account newAccount = new Account()
            {
                User_Id = Convert.ToInt32(reader["user_id"]),
                Account_Id = Convert.ToInt32(reader["account_id"]),
                Balance = Convert.ToDecimal(reader["ballance"])
            };

            return newAccount;
        }

        public Account Deposit(Account account, decimal amountToDeposit)
        {
            account.Balance += amountToDeposit;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string sqlText = "Update accounts set balance = @newBalance " +
                        "where account_id = @accountId;";
                    SqlCommand cmd = new SqlCommand(sqlText, conn);
                    cmd.Parameters.AddWithValue("@newBalance", account.Balance);
                    cmd.Parameters.AddWithValue("@accountId", account.Account_Id);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return account;
        }

        public Account Withdraw(Account account, decimal amountToWidthdraw)
        {
            account.Balance -= amountToWidthdraw;
            return account;
        }
    }
}
