using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.Models
{
    public class Transfer
    {
        public int TransferId { get; set; }
        [Required(ErrorMessage = "Must include transfer type.")]
        public string TransferType { get; set; }
        [Required(ErrorMessage = "Must include a status for the transfer.")]
        public string TransferStatus { get; set; }
        public string UserFrom { get; set; }
        [Required(ErrorMessage = "Must include a user to transfer from.")]
        public int UserFromId { get; set; }
        public string UserTo { get; set; }
        [Required(ErrorMessage = "Must include a user to transfer to.")]
        public int UserToId { get; set; }
        [Required(ErrorMessage = "Must include an amount to transfer.")]
        public decimal Amount { get; set; }
    }
}
