using System;
using System.Collections.Generic;
using System.Text;

namespace TenmoClient.Data
{
    public class API_Transfer
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
