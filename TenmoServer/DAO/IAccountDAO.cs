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
        bool Deposit(Account account, decimal amountToDeposit);
        bool Withdraw(Account account, decimal amountToWidthdraw);

    }
}
