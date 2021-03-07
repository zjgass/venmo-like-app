using System;
using System.Collections.Generic;
using System.Text;
using TenmoClient.Data;

namespace TenmoClient
{
    public class TUI
    {
        private readonly string API_BASE_URL = "https://localhost:44315/";
        private static readonly ConsoleService consoleService = new ConsoleService();
        private static readonly AuthService authService = new AuthService();
        private readonly AccountService accountService = new AccountService();
        private readonly TransferService transferService = new TransferService();

        public void Run()
        {
            int loginRegister = -1;
            while (loginRegister != 1 && loginRegister != 2)
            {
                Console.WriteLine("Welcome to TEnmo!");
                Console.WriteLine("1: Login");
                Console.WriteLine("2: Register");
                Console.Write("Please choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out loginRegister))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else if (loginRegister == 1)
                {
                    while (!UserService.IsLoggedIn()) //will keep looping until user is logged in
                    {
                        LoginUser loginUser = consoleService.PromptForLogin();
                        API_User user = authService.Login(loginUser);
                        if (user != null)
                        {
                            UserService.SetLogin(user);
                        }
                    }
                }
                else if (loginRegister == 2)
                {
                    bool isRegistered = false;
                    while (!isRegistered) //will keep looping until user is registered
                    {
                        LoginUser registerUser = consoleService.PromptForLogin();
                        isRegistered = authService.Register(registerUser);
                        if (isRegistered)
                        {
                            Console.WriteLine("");
                            Console.WriteLine("Registration successful. You can now log in.");
                            loginRegister = -1; //reset outer loop to allow choice for login
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid selection.");
                }
            }

            MenuSelection();
        }

        private void MenuSelection()
        {
            const int getCurrentBalance = 1;
            const int pastTransactions = 2;
            const int pendingRequests = 3;
            const int send = 4;
            const int request = 5;
            const int logOut = 6;

            int menuSelection = -1;
            while (menuSelection != 0)
            {
                Console.WriteLine("");
                Console.WriteLine("Welcome to TEnmo! Please make a selection: ");
                Console.WriteLine("1: View your current balance");
                Console.WriteLine("2: View your past transfers");
                Console.WriteLine("3: View your pending requests");
                Console.WriteLine("4: Send TE bucks");
                Console.WriteLine("5: Request TE bucks");
                Console.WriteLine("6: Log in as different user");
                Console.WriteLine("0: Exit");
                Console.WriteLine("---------");
                Console.Write("Please choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out menuSelection))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else if (menuSelection == getCurrentBalance)
                {
                    API_Account currentBalance = accountService.GetAccountBalance();
                    Console.WriteLine("Balance: " + currentBalance.Balance);
                }
                else if (menuSelection == pastTransactions)
                {
                    List<API_Transfer> pastTransfers = transferService.GetPastTransfers();
                    foreach(API_Transfer transfer in pastTransfers)
                    {
                        Console.WriteLine($"| {transfer.TransferId.ToString().PadRight(5)} | {transfer.TransferStatus.ToString().PadRight(10)} | {transfer.UserFrom.ToString().PadRight(20)} | {transfer.UserTo.ToString().PadRight(20)} | {transfer.Amount.ToString().PadRight(6)}");
                    }
                }
                else if (menuSelection == pendingRequests)
                {
                    List<API_Transfer> pendingTransfers = transferService.GetPendingTransers();
                    foreach (API_Transfer transfer in pendingTransfers)
                    {
                        Console.WriteLine($"| {transfer.TransferId.ToString().PadRight(5)} | {transfer.TransferStatus.ToString().PadRight(10)} | {transfer.UserFrom.ToString().PadRight(20)} | {transfer.UserTo.ToString().PadRight(20)} | {transfer.Amount.ToString().PadRight(6)}");
                    }
                }
                else if (menuSelection == send)
                {
                    int userID;
                    decimal amount;

                    List<API_User> otherUsers = accountService.GetAllUsers();
                    Console.WriteLine("ID       | Name");
                    Console.WriteLine("----------------------------------------------------");
                    foreach (API_User user in otherUsers)
                    {
                        Console.WriteLine($"| {user.UserId.ToString().PadRight(5)} | {user.Username.ToString().PadRight(20)}");
                    }

                    try
                    {
                        bool validInput = false;
                        do
                        {
                            Console.Write("Please enter the Account Number for the Account you wish to send to: ");
                            string _userID = Console.ReadLine();
                            Console.Write("Please Enter the amount you wish to send: ");
                            string _amount = Console.ReadLine();
                            userID = int.Parse(_userID);
                            amount = decimal.Parse(_amount);
                            if(userID != 0 && amount != 0)
                            {
                                validInput = true;
                            }
                            else
                            {
                                Console.WriteLine("Please enter a valid User ID and Amount");
                            }
                        } while (!validInput);
                        
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    
                 
                    API_Transfer sendTransfer = transferService.SendTEbucks(userID,amount);
                    Console.WriteLine($"| {sendTransfer.TransferId.ToString().PadRight(5)} | {sendTransfer.UserFrom.ToString().PadRight(20)} | {sendTransfer.UserTo.ToString().PadRight(20)} | {sendTransfer.Amount.ToString().PadRight(6)}");
                }
                else if (menuSelection == request)
                {
                    int userID;
                    decimal amount;
                    try
                    {
                        bool validInput = false;
                        do
                        {
                            Console.Write("Please enter the Account Number for the Account you wish to Request from: ");
                            string _userID = Console.ReadLine();
                            Console.Write("Please Enter the amount you wish to Request: ");
                            string _amount = Console.ReadLine();
                            userID = int.Parse(_userID);
                            amount = decimal.Parse(_amount);
                            if (userID != 0 && amount != 0)
                            {
                                validInput = true;
                            }
                            else
                            {
                                Console.WriteLine("Please enter a valid User ID and Amount");
                            }
                        } while (!validInput);

                    }
                    catch (Exception)
                    {

                        throw;
                    }


                    API_Transfer requestTransfer = transferService.RequestTransfer(userID, amount);
                    Console.WriteLine($"| {requestTransfer.TransferId.ToString().PadRight(5)} | {requestTransfer.TransferStatus.ToString().PadRight(10)} | {requestTransfer.UserFrom.ToString().PadRight(20)} | {requestTransfer.UserTo.ToString().PadRight(20)} | {requestTransfer.Amount.ToString().PadRight(6)}");
                }
                else if (menuSelection == logOut)
                {
                    Console.WriteLine("");
                    UserService.SetLogin(new API_User()); //wipe out previous login info
                    Run();
                }
                else
                {
                    Console.WriteLine("Goodbye!");
                    Environment.Exit(0);
                }
            }
        }
    }
}
