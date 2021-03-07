using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
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
            Account accountInfo = new Account();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT account_id, user_id, balance FROM accounts WHERE user_id = @userid", conn);
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
                UserId = Convert.ToInt32(reader["user_id"]),
                AccountId = Convert.ToInt32(reader["account_id"]),
                Balance = Convert.ToDecimal(reader["balance"])
            };

            return newAccount;
        }

        public bool Deposit(Account account, decimal amountToDeposit)
        {
            account.Balance += amountToDeposit;
            return HelperUpdateBalance(account.Balance, account.AccountId);
        }

        public bool Withdraw(Account account, decimal amountToWidthdraw)
        {
            account.Balance -= amountToWidthdraw;
            return HelperUpdateBalance(account.Balance, account.AccountId);
        }

        private bool HelperUpdateBalance(decimal amount, int accountId)
        {
            bool successful = false;

            try
            {
                //using (TransactionScope transaction = new TransactionScope())
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string sqlText = "update accounts set balance = @amount " +
                                    "where account_id = @accountId;";
                    SqlCommand cmd = new SqlCommand(sqlText, conn);
                    cmd.Parameters.AddWithValue("@amount", amount);
                    cmd.Parameters.AddWithValue("@accountId", accountId);

                    successful = true;
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return successful;
        }
    }
}
