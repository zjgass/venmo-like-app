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
        public int Account_Id { get; set; }
        [Required(ErrorMessage = "Must include a user id.")]
        public int User_Id { get; set; }
        public decimal Balance { get; set; }
    }
}
