using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.Models
{
    public class Account
    {
        [Required(ErrorMessage = "Must include an account id.")]
        public int AccountId { get; set; }
        [Required(ErrorMessage = "Must include a user id.")]
        public int UserId { get; set; }
        public decimal Balance { get; set; }
    }
}
