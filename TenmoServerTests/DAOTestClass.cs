using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data.SqlClient;
using System.Transactions;
using TenmoServer.Models;
using TenmoServer.Security;
using TenmoServer.Security.Models;

namespace TenmoServerTests.TransferSqlDAOTests
{
    [TestClass]
    public class DAOTestClass
    {
        public static IConfiguration Configuration
        {
            get
            {
                var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .Build();
                return config;
            }
        }
        protected TransactionScope Transaction { get; set; }
        protected string connectionString = Configuration.GetConnectionString("Project");
        protected User TestUser1 { get; set; } = new User();
        protected User TestUser2 { get; set; } = new User();
        protected Account TestAccount1 { get; set; } = new Account();
        protected Account TestAccount2 { get; set; } = new Account();
        protected Transfer TestTransfer { get; set; } = new Transfer();

        [TestInitialize]
        public void SetUp()
        {
            Transaction = new TransactionScope();
            IPasswordHasher passwordHasher = new PasswordHasher();
            PasswordHash hash = passwordHasher.ComputeHash("password");
            const decimal initialBalance = 1000;

            // Define TestUser1
            TestUser1.Username = "TestUser1";
            TestUser1.PasswordHash = hash.Password;
            TestUser1.Salt = hash.Salt;

            // Define TestUser2
            TestUser2.Username = "TestUser2";
            TestUser2.PasswordHash = hash.Password;
            TestUser2.Salt = hash.Salt;

            // Define TestTrnsfer
            TestTransfer.TransferType = "send";
            TestTransfer.TransferStatus = "pending";
            TestTransfer.Amount = 50.00M;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Insert the Test Users into the database.
                    string sqlText = "insert into users (username, password_hash, salt) " +
                        "values (@username, @hash, @salt); select scope_Identity();";
                    SqlCommand cmd = new SqlCommand(sqlText, conn);
                    cmd.Parameters.AddWithValue("@username", TestUser1.Username);
                    cmd.Parameters.AddWithValue("@hash", TestUser1.PasswordHash);
                    cmd.Parameters.AddWithValue("@salt", TestUser1.Salt);
                    TestUser1.UserId = Convert.ToInt32(cmd.ExecuteScalar());

                    cmd = new SqlCommand(sqlText, conn);
                    cmd.Parameters.AddWithValue("@username", TestUser2.Username);
                    cmd.Parameters.AddWithValue("@hash", TestUser2.PasswordHash);
                    cmd.Parameters.AddWithValue("@salt", TestUser2.Salt);
                    TestUser2.UserId = Convert.ToInt32(cmd.ExecuteScalar());

                    // Create new accounts for the Test Users.
                    sqlText = "insert into accounts (user_id, balance) " +
                        "values ( (select user_id from users where username = @username), @balance);";
                    cmd = new SqlCommand(sqlText, conn);
                    cmd.Parameters.AddWithValue("@username", TestUser1.Username);
                    cmd.Parameters.AddWithValue("@balance", initialBalance);
                    cmd.ExecuteNonQuery();

                    cmd = new SqlCommand(sqlText, conn);
                    cmd.Parameters.AddWithValue("@username", TestUser2.Username);
                    cmd.Parameters.AddWithValue("@balance", initialBalance);
                    cmd.ExecuteNonQuery();

                    // Create some transfers for testing.
                    sqlText = "insert into transfers (transfer_type_id, transfer_status_id, account_from, account_to, amount) " +
                        "values ( (select transfer_type_id from transfer_types where transfer_type_desc = @type), " +
                        "(select transfer_status_id from transfer_statuses where transfer_status_desc = @status), " +
                        "(select account_id from accounts where user_id = @userFromId), " +
                        "(select account_id from accounts where user_id = @userToId), " +
                        "@amount ); select scope_Identity();";
                    cmd = new SqlCommand(sqlText, conn);
                    cmd.Parameters.AddWithValue("@type", TestTransfer.TransferType);
                    cmd.Parameters.AddWithValue("@status", TestTransfer.TransferStatus);
                    cmd.Parameters.AddWithValue("@userFromId", TestUser1.UserId);
                    cmd.Parameters.AddWithValue("@userToId", TestUser2.UserId);
                    cmd.Parameters.AddWithValue("@amount", TestTransfer.Amount);
                    TestTransfer.TransferId = Convert.ToInt32(cmd.ExecuteScalar());

                    cmd = new SqlCommand(sqlText, conn);
                    cmd.Parameters.AddWithValue("@type", "send");
                    cmd.Parameters.AddWithValue("@status", "approved");
                    cmd.Parameters.AddWithValue("@userFromId", TestUser2.UserId);
                    cmd.Parameters.AddWithValue("@userToId", TestUser1.UserId);
                    cmd.Parameters.AddWithValue("@amount", 75);
                    cmd.ExecuteNonQuery();

                    cmd = new SqlCommand(sqlText, conn);
                    cmd.Parameters.AddWithValue("@type", "request");
                    cmd.Parameters.AddWithValue("@status", "pending");
                    cmd.Parameters.AddWithValue("@userFromId", TestUser1.UserId);
                    cmd.Parameters.AddWithValue("@userToId", TestUser2.UserId);
                    cmd.Parameters.AddWithValue("@amount", 50);
                    cmd.ExecuteNonQuery();

                    cmd = new SqlCommand(sqlText, conn);
                    cmd.Parameters.AddWithValue("@type", "request");
                    cmd.Parameters.AddWithValue("@status", "rejected");
                    cmd.Parameters.AddWithValue("@userFromId", TestUser1.UserId);
                    cmd.Parameters.AddWithValue("@userToId", TestUser2.UserId);
                    cmd.Parameters.AddWithValue("@amount", 50);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [TestCleanup]
        public void CleanUp()
        {
            Transaction.Dispose();
        }
    }
}
