using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.Models
{
    public class Transfer
    {
        public int TransferId { get; set; }
        public string TransferType { get; set; }
        public string TransferStatus { get; set; }
        public string UserFrom { get; set; }
        public int UserFromId { get; set; }
        public string UserTo { get; set; }
        public int UserToId { get; set; }
        public decimal Amount { get; set; }
    }
}
