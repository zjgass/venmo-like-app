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
    public class NewTransferTests : DAOTestClass
    {
        [TestMethod]
        public void HappyPath()
        {
            // Arrange

        }

        public void GetNumberOfTransfers()
        {
            try
            {
                using(SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand($"select count(*) from transfers " +
                        "where (account_to = (select account_id from accounts where user_id = @userId) " +
                        "or (account_from = (select account_id from accounts where user_id = @userId); " +
                        "select scope_identity();");
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }
    }
}
