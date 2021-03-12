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
        [Range(1, Int32.MaxValue)]
        public int UserFromId { get; set; }
        public string UserTo { get; set; }
        [Required(ErrorMessage = "Must include a user to transfer to.")]
        [Range(1, Int32.MaxValue)]
        public int UserToId { get; set; }
        [Required(ErrorMessage = "Must include an amount to transfer.")]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Transfer t = (Transfer)obj;
                return (TransferId == t.TransferId)
                    && (TransferType.ToLower().Trim().Equals(t.TransferType.ToLower().Trim()))
                    && (TransferStatus.ToLower().Trim().Equals(t.TransferStatus.ToLower().Trim()))
                    && (UserFromId == t.UserFromId)
                    && (UserToId == t.UserToId)
                    && (Amount == t.Amount);
            }
        }
    }
}
