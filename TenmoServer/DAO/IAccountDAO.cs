using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface IAccountDAO
    {
        Account GetAccount(int userId);
<<<<<<< HEAD
=======
        Account Deposit(Account account, decimal amountToDeposit);
        Account Withdraw(Account account, decimal amountToWidthdraw);
>>>>>>> 51e05b50f7eb0b8036954d73b466127ddc3c1811
    }
}
