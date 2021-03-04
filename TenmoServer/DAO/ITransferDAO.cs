using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface ITransferDAO
    {
        Transfer NewTransfer(Transfer transfer);
        bool UpdateTransfer(Transfer transfer);
        List<Transfer> GetAllTransfers(string userName, bool areComplete);
        Transfer GetTransfer(string userName, int transferId);
    }
}
