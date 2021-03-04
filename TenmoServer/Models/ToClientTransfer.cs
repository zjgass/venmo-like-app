using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.Models
{
    /// <summary>
    /// A transfer with the information that the client
    /// gets back from the server.
    /// </summary>
    public class ToClientTransfer
    {
        public int TransferId { get; set; }
        public string TransferType { get; set; }
        public string TransferStatus { get; set; }
        public string UserFrom { get; set; }
        public string UserTo { get; set; }
        public decimal Amount { get; set; }
    }

    /// <summary>
    /// A transfer with the information that the client
    /// sends to the server.
    /// </summary>
    public class FromClientTransfer
    {
        public int TransferId { get; set; }
        public string TransferType { get; set; }
        public string TransferStatus { get; set; }
        public string Author { get; set; }
        public int AddresseeId { get; set; }
        public decimal Amount { get; set; }
    }
}
