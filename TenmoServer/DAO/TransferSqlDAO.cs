using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public class TransferSqlDAO : ITransferDAO
    {
        private readonly string connectionString;

        public TransferSqlDAO(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public bool ExecuteTransfre()
        {
            throw new NotImplementedException();
        }

        public List<Transfer> GetAllTransfers(int userId)
        {
            List<Transfer> transfers = new List<Transfer>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("select transfer_id, transfer_type_id, " +
                        "transfer_status_id, account_from, account_to from transfers " +
                        "where account_from = @users_account or account_to = @users_account;");

                }
            }
            catch (Exception)
            {

                throw;
            }

            return transfers;
        }

        public Transfer GetPendingTransfer()
        {
            throw new NotImplementedException();
        }

        public Transfer GetTransfer()
        {
            throw new NotImplementedException();
        }

        public Transfer RequestTransfer()
        {
            throw new NotImplementedException();
        }

        public Transfer SendTransfer()
        {
            throw new NotImplementedException();
        }
    }
}
