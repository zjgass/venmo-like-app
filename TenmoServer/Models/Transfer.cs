using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.Models
{
    public class Transfer
    {
        private int TransferId { get; }
        private string TransferType { get; set; }
        private string TransferStatus { get; set; }
        private string UserFrom { get; set; }
        private string UserTo { get; set; }
        private decimal Amount { get; set; }
    }
}
