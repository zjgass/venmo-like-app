using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface ITransferDAO
    {
        Transfer SendTransfer(string userfrom, int userToId, decimal amount);
        bool ExecuteTransfre();
        Transfer RequestTransfer();
        List<Transfer> GetAllTransfers(string userName, bool areComplete);
        Transfer GetTransfer(string userName, int transferId);
    }
}
