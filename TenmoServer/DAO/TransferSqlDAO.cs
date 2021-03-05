﻿using System;
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

        public List<Transfer> GetAllTransfers(int userId, bool areComplete)
        {
            List<Transfer> transfers = new List<Transfer>() { };

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string sqlText = "select transfer_id, type.transfer_type_desc, status.transfer_status_desc, " +
                        "userfrom.username as userfrom, userfrom.user_id as userfromid, " +
                        "userto.username as userto, userto.user_id as usertoid, amount " +
                        "from transfers " +
                        "join transfer_types as type on transfers.transfer_type_id = type.transfer_type_id " +
                        "join transfer_statuses as status on transfers.transfer_status_id = status.transfer_status_id " +
                        "join accounts as accfrom on transfers.account_from = accfrom.account_id " +
                        "join accounts as accto on transfers.account_to = accto.account_id " +
                        "join users as userfrom on accfrom.user_id = userfrom.user_id " +
                        "join users as userto on accto.user_id = userto.user_id " +
                        "where (userfrom.user_id = @userId " +
                        "or userto.user_id = @userId) ";
                    sqlText += "and status.transfer_status_desc " + (areComplete ? "!= 'pending';" : "= 'pending';");
                    SqlCommand cmd = new SqlCommand(sqlText, conn);
                    cmd.Parameters.AddWithValue("@userId", userId);
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

        public Transfer GetTransfer(int userId, int transferId)
        {
            Transfer transfer = new Transfer();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string sqlText = ("select transfer_id, type.transfer_type_desc, status.transfer_status_desc, " +
                        "userfrom.username as userfrom, userfrom.user_id as userfromid, " +
                        "userto.username as userto, userto.user_id as usertoid, amount " +
                        "from transfers " +
                        "join transfer_types as type on transfers.transfer_type_id = type.transfer_type_id " +
                        "join transfer_statuses as status on transfers.transfer_status_id = status.transfer_status_id " +
                        "join accounts as accfrom on transfers.account_from = accfrom.account_id " +
                        "join accounts as accto on transfers.account_to = accto.account_id " +
                        "join users as userfrom on accfrom.user_id = userfrom.user_id " +
                        "join users as userto on accto.user_id = userto.user_id " +
                        "where (userfrom.user_id = @userId " +
                        "or userto.user_id = @userId) " +
                        "and transfer_id = @transferId;");
                    SqlCommand cmd = new SqlCommand(sqlText, conn);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@transferId", transferId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows && reader.Read())
                    {
                        transfer = ConvertReaderToTransfer(reader);
                    }
                }
            }
            catch (Exception e)
            {

                throw e;
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

                    string sqlText = "insert into transfers (transfer_type_id, transfer_status_id, account_from, account_to, amount) " +
                        "values ((select transfer_type_id from transfer_types where transfer_type_desc = @transferType), " +
                        "(select transfer_status_id from transfer_statuses where transfer_status_desc = @transferStatus)," +
                        "(select account_id from accounts where user_id = @userFromId), " +
                        "(select account_id from accounts where user_id = @userToId), " +
                        "@amount); " +
                        "select scope_Identity();";
                    SqlCommand cmd = new SqlCommand(sqlText, conn);
                    cmd.Parameters.AddWithValue("@transferType", transfer.TransferType);
                    cmd.Parameters.AddWithValue("@transferStatus", transfer.TransferStatus);
                    cmd.Parameters.AddWithValue("@userFromId", transfer.UserFromId);
                    cmd.Parameters.AddWithValue("@userToId", transfer.UserToId);
                    cmd.Parameters.AddWithValue("@amount", transfer.Amount);

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
                UserFrom = Convert.ToString(reader["userfrom"]),
                UserFromId = Convert.ToInt32(reader["userfromid"]),
                UserTo = Convert.ToString(reader["userto"]),
                UserToId = Convert.ToInt32(reader["usertoid"]),
                Amount = Convert.ToDecimal(reader["amount"])
            };

            return transfer;
        }
    }
}
