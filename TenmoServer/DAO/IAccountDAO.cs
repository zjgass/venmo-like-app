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
        Account Deposit(Account account, decimal amountToDeposit);
        Account Withdraw(Account account, decimal amountToWidthdraw);

    }
}
