using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public class TransferSqlDAO : ITransferDAO
    {
        private readonly string connectionString;
        private TransactionScope transaction { get; set; }

        public TransferSqlDAO(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public bool UpdateTransfer(FromClientTransfer transfer) // TODO 01 Change to from client.
        {
            bool updateComplete = false;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string sqlText = "Update transfers set transfer_status_id = " +
                        "(select transfer_status_id from transfer_statuses where transfer_status_desc = @status) " +
                        "where transfer_id = @transferId and transfer_status_id = " +
                        "(select transfer_status_id from transfer_statuses where transfer_status_desc = 'pending');";
                    SqlCommand cmd = new SqlCommand(sqlText, conn);
                    cmd.Parameters.AddWithValue("@status", transfer.TransferStatus);
                    cmd.Parameters.AddWithValue("@transferId", transfer.TransferId);
                    int rowsAffected = Convert.ToInt32(cmd.ExecuteNonQuery());

                    if (rowsAffected > 0)
                    {
                        updateComplete = true;
                    }
                }
            }
            catch (Exception e)
            {

                throw e;
            }

            return updateComplete;
        }

        public List<ToClientTransfer> GetAllTransfers(string userName, bool areComplete)
        {
            List<ToClientTransfer> transfers = new List<ToClientTransfer>() { };

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string sqlText = "select transfer_id, type.transfer_type_desc, status.transfer_status_desc, " +
                        "userfrom.username, userto.username, amount " +
                        "from transfers " +
                        "join transfer_types as type on transfers.transfer_type_id = type.transfer_type_id " +
                        "join transfer_statuses as status on transfers.transfer_status_id = status.transfer_status_id " +
                        "join accounts as accfrom on transfers.account_from = accfrom.account_id " +
                        "join accounts as accto on transfers.account_to = accto.account_id " +
                        "join users as userfrom on accfrom.user_id = userfrom.user_id " +
                        "join users as userto on accto.user_id = userto.user_id " +
                        "where (userfrom.user_id = (select user_id from users where username = @userName) " +
                        "or userto.user_id = (select user_id from users where username = @userName)) ";
                    sqlText += "and status.transfer_status_desc " + (areComplete ? "= 'approved';" : "= 'pending';");
                    SqlCommand cmd = new SqlCommand(sqlText, conn);
                    cmd.Parameters.AddWithValue("@userName", userName);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        transfers.Add(ConvertReaderToTransfer(reader));
                    }
                }
            }
            catch (Exception e)
            {

                throw e;
            }

            return transfers;
        }

        public ToClientTransfer GetTransfer(string userName, int transferId)
        {
            ToClientTransfer transfer = new ToClientTransfer();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string sqlText = ("select transfer_id, type.transfer_type_desc, status.transfer_status_desc, " +
                        "userfrom.username, userto.username, amount " +
                        "from transfers " +
                        "join transfer_types as type on transfers.transfer_type_id = type.transfer_type_id " +
                        "join transfer_statuses as status on transfers.transfer_status_id = status.transfer_status_id " +
                        "join accounts as accfrom on transfers.account_from = accfrom.account_id " +
                        "join accounts as accto on transfers.account_to = accto.account_id " +
                        "join users as userfrom on accfrom.user_id = userfrom.user_id " +
                        "join users as userto on accto.user_id = userto.user_id " +
                        "where (userfrom.user_id = (select user_id from users where username = @userName) " +
                        "or userto.user_id = (select user_id from users where username = @userName)) " +
                        "and transfer_id = @transferId;");
                    SqlCommand cmd = new SqlCommand(sqlText, conn);
                    cmd.Parameters.AddWithValue("@userName", userName);
                    cmd.Parameters.AddWithValue("@transferId", transferId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows && reader.Read())
                    {
                        transfer = ConvertReaderToTransfer(reader);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

            return transfer;
        }

        public ToClientTransfer NewTransfer(FromClientTransfer transfer) //TODO 02 Change to from client.
        {
            ToClientTransfer transferOut = new ToClientTransfer();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string sqlText = "insert into transfers (transfer_type_id, transfer_status_id, account_from, account_to, amount) " +
                        "values ((select transfer_type_id from transfer_types where transfer_type_desc = @transferType), " +
                        "(select transfer_status_id from transfer_statuses where transfer_status_desc = @transferStatus), ";
                    if (transfer.TransferType.ToLower().Equals("send"))
                    {
                        sqlText += "(select account_id from accounts where user_id = (select user_id from users where username = @author))," +
                            "(select account_id from accounts where user_id = @addresseeId));";
                    }
                    else
                    {
                        sqlText += "(select account_id from accounts where user_id = @addresseeId) " +
                            "(select account_id from accounts where user_id = (select user_id from users where username = @author));";
                    }
                    sqlText += " select scope_Identity();";
                    SqlCommand cmd = new SqlCommand(sqlText, conn);
                    cmd.Parameters.AddWithValue("@transferType", transfer.TransferType);
                    cmd.Parameters.AddWithValue("@transferStatus", transfer.TransferStatus);
                    cmd.Parameters.AddWithValue("@author", transfer.Author);
                    cmd.Parameters.AddWithValue("@addresseeId", transfer.AddresseeId);

                    transfer.TransferId = Convert.ToInt32(cmd.ExecuteScalar());

                    transferOut = GetTransfer(transfer.Author, transfer.TransferId);
                }
            }
            catch (Exception e)
            {

                throw e;
            }

            return transferOut;
        }

        private ToClientTransfer ConvertReaderToTransfer(SqlDataReader reader)
        {
            ToClientTransfer transfer = new ToClientTransfer()
            {
                TransferId = Convert.ToInt32(reader["transfer_id"]),
                TransferType = Convert.ToString(reader["transfer_type_desc"]),
                TransferStatus = Convert.ToString(reader["transfer_status_desc"]),
                UserFrom = Convert.ToString(reader["username"]),
                UserTo = Convert.ToString(reader["username"]),
                Amount = Convert.ToDecimal(reader["amount"])
            };

            return transfer;
        }

        private bool ExecuteTransfer(ToClientTransfer transfer)
        {
            bool txComplete = false;

            if (transfer.TransferStatus.ToLower().Equals("approved"))
            {
                decimal balance = 0;
                try
                {
                    using(SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        string sqlText = "select balance from accounts where user_id = " +
                            "(select user_id from users where username = @userFrom); " +
                            "select scope_Identity();";
                        SqlCommand cmd = new SqlCommand(sqlText, conn);
                        cmd.Parameters.AddWithValue("@userFrom", transfer.UserFrom);

                        balance = Convert.ToDecimal(cmd.ExecuteScalar());
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }

                if (transfer.Amount <= balance)
                {
                    transaction = new TransactionScope();

                    decimal initSum = 0;

                    try
                    {
                        using (SqlConnection conn = new SqlConnection(connectionString))
                        {
                            conn.Open();

                            // This is to check the initial sum of the balances. If this does not match the balance
                            // at the end the transaction will be rolled back.
                            string sqlText = "select sum(balance) " +
                                "from accounts " +
                                "where user_id = (select user_id from users where username = @userFrom " +
                                "and user_id = (select user_id from users where username = @userTo); " +
                                "select scope_Itenditiy();";
                            SqlCommand cmd = new SqlCommand(sqlText, conn);
                            cmd.Parameters.AddWithValue("@addresseeId", transfer.UserFrom);
                            cmd.Parameters.AddWithValue("@author", transfer.UserTo);
                            initSum = Convert.ToDecimal(cmd.ExecuteScalar());

                            // Now we begin the withdrawl.

                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
            }

            return txComplete;
        }
    }
}
