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

        public bool UpdateTransfer(Transfer transfer)
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

        public List<Transfer> GetAllTransfers(string userName, bool areComplete)
        {
            List<Transfer> transfers = new List<Transfer>() { };

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

        public Transfer GetTransfer(string userName, int transferId)
        {
            Transfer transfer = new Transfer();

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

        public Transfer NewTransfer(Transfer transfer)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string sqlText = "select user_id from users where user_id = @userFromId or user_id = @userToId;";
                    SqlCommand cmd = new SqlCommand(sqlText, conn);
                    cmd.Parameters.AddWithValue("@userId", transfer.UserTo);

                    sqlText = "insert into transfers (transfer_type_id, transfer_status_id, account_from, account_to, amount) " +
                        "values ((select transfer_type_id from transfer_types where transfer_type_desc = @transferType), " +
                        "(select transfer_status_id from transfer_statuses where transfer_status_desc = @transferStatus), " +
                        "(select account_id from accounts where user_id = (select user_id from users where username = @userFrom))," +
                        "(select account_id from accounts where user_id = @userToId)); select scope_Identity();";
                    cmd = new SqlCommand(sqlText, conn);
                    cmd.Parameters.AddWithValue("@transferType", transfer.TransferType);
                    cmd.Parameters.AddWithValue("@transferStatus", transfer.TransferStatus);
                    cmd.Parameters.AddWithValue("@userFrom", transfer.Author);
                    cmd.Parameters.AddWithValue("@userToId", transfer.UserToId);

                    transfer.TransferId = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception e)
            {

                throw e;
            }

            return transfer;
        }

        private Transfer ConvertReaderToTransfer(SqlDataReader reader)
        {
            Transfer transfer = new Transfer()
            {
                TransferId = Convert.ToInt32(reader["transfer_id"]),
                TransferType = Convert.ToString(reader["transfer_type_desc"]),
                TransferStatus = Convert.ToString(reader["transfer_status_desc"]),
                Author = Convert.ToString(reader["username"]),
                UserTo = Convert.ToString(reader["username"]),
                Amount = Convert.ToDecimal(reader["amount"])
            };

            return transfer;
        }

        private bool ExecuteTransfer(Transfer transfer)
        {
            bool txComplete = false;

            if (transfer.TransferStatus.ToLower().Equals("approved"))
            {
                transaction = new TransactionScope();

                decimal initSum = 0;

                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        string sqlText = "select sum(balance) " +
                            "from accounts " +
                            "where user_id = ";
                        sqlText += (transfer.UserFromId > 0) ? "@userFromId " : "(select user_id from users where username = @userFrom) ";
                        sqlText += "or user_id = ";
                        sqlText += (transfer.UserToId > 0) ? "@userToId " : "(select user_id from users where username = @userTo); ";
                        sqlText+= "select scope_Identitiy;";
                        SqlCommand cmd = new SqlCommand(sqlText, conn);
                        if (transfer.UserFromId > 0)
                        {
                            cmd.Parameters.AddWithValue("@userFromId", transfer.UserFromId);
                        } else
                        {
                            cmd.Parameters.AddWithValue("@userFrom", transfer.Author);
                        }
                        if (transfer.UserToId > 0)
                        {
                            cmd.Parameters.AddWithValue("@userToId", transfer.UserToId);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@userTo", transfer.UserTo);
                        }

                        initSum = Convert.ToDecimal(cmd.ExecuteScalar());


                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

            return txComplete;
        }
    }
}
