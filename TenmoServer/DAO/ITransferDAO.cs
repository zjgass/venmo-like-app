using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface ITransferDAO
    {
        ToClientTransfer NewTransfer(FromClientTransfer transfer);
        bool UpdateTransfer(FromClientTransfer transfer);
        List<ToClientTransfer> GetAllTransfers(string userName, bool areComplete);
        ToClientTransfer GetTransfer(string userName, int transferId);
    }
}
