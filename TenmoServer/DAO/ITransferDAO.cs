using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface ITransferDAO
    {
        List<Transfer> GetAllTransfers(int userId, bool areComplete);
        Transfer GetTransfer(int userId, int transferId);
        Transfer NewTransfer(Transfer transfer);
        Transfer UpdateTransfer(Transfer transfer);
    }
}
