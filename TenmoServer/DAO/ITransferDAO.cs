using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface ITransferDAO
    {
        Transfer SendTransfer();
        bool ExecuteTransfre();
        Transfer RequestTransfer();
        List<Transfer> GetAllTransfers(int userId);
        Transfer GetTransfer();
        Transfer GetPendingTransfer();
    }
}
