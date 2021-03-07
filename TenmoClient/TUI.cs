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
            const string id = "ID";
            const string name = "Name";
            const string userFrom = "UserFrom";
            const string userTo = "UserTo";
            const string sentAmount = "Amount";
            const string status = "Status";

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
                    Console.WriteLine($"| {id.PadRight(5)} | {status.PadRight(10)} | {userFrom.PadRight(20)} | {userTo.PadRight(20)} | {sentAmount.PadRight(6)}");
                    Console.WriteLine("------------------------------------------------------------");
                    foreach (API_Transfer transfer in pastTransfers)
                    {
                        Console.WriteLine($"| {transfer.TransferId.ToString().PadRight(5)} | {transfer.TransferStatus.ToString().PadRight(10)} | {transfer.UserFrom.ToString().PadRight(20)} | {transfer.UserTo.ToString().PadRight(20)} | {transfer.Amount.ToString().PadRight(6)}");
                    }
                }
                else if (menuSelection == pendingRequests)
                {
                    List<API_Transfer> pendingTransfers = transferService.GetPendingTransers();
                    Console.WriteLine($"| {id.PadRight(5)} | {status.PadRight(10)} | {userFrom.PadRight(20)} | {userTo.PadRight(20)} | {sentAmount.PadRight(6)}");
                    Console.WriteLine("------------------------------------------------------------");
                    foreach (API_Transfer transfer in pendingTransfers)
                    {
                        Console.WriteLine($"| {transfer.TransferId.ToString().PadRight(5)} | {transfer.TransferStatus.ToString().PadRight(10)} | {transfer.UserFrom.ToString().PadRight(20)} | {transfer.UserTo.ToString().PadRight(20)} | {transfer.Amount.ToString().PadRight(6)}");
                    }

                    bool continueWorking = true;
                    API_Transfer updatedTransfer = new API_Transfer();
                    try
                    {
                        do 
                        {
                            Console.Write("Would you like to Approve/Reject a request?(0 to go back to the menu): ");
                            string transferString = Console.ReadLine();
                            int transferNum = int.Parse(transferString);
                            foreach (API_Transfer transfers in pendingTransfers)
                            {
                                if (transfers.TransferId == transferNum)
                                {
                                    Console.WriteLine("1: Approve\n"+
                                                      "2: Reject\n" +
                                                      "0: Leave as Pending");
                                    Console.WriteLine("Please select an option: ");
                                    string option = Console.ReadLine();
                                    if(option.Trim() == "1")
                                    {
                                        option = "Approved";
                                        updatedTransfer = transferService.UpdateTransfer(transfers, option);
                                    } else if (option.Trim() == "2")
                                    {
                                        option = "Reject";
                                        updatedTransfer = transferService.UpdateTransfer(transfers, option);
                                    } else if (option.Trim() == "0")
                                    {
                                        option = "end";
                                    }
                                    
                                }
                                else if (transferNum == 0)
                                {
                                    continueWorking = false;
                                }
                                else
                                {
                                    updatedTransfer = null;
                                }
                            }
                            if (updatedTransfer != null)
                            {
                                Console.WriteLine("Updated Transfer Request:");
                                Console.WriteLine($"| {id.PadRight(5)} | {status.PadRight(10)} | {userFrom.PadRight(20)} | {userTo.PadRight(20)} | {sentAmount.PadRight(6)}");
                                Console.WriteLine("------------------------------------------------------------");
                                Console.WriteLine($"| {updatedTransfer.TransferId.ToString().PadRight(5)} | {updatedTransfer.TransferStatus.ToString().PadRight(10)} | {updatedTransfer.UserFrom.ToString().PadRight(20)} | {updatedTransfer.UserTo.ToString().PadRight(20)} | {updatedTransfer.Amount.ToString().PadRight(6)}");
                            }
                            else
                            {
                                Console.WriteLine("Sorry, We couldn't find that account, Please try again");
                            }
                        } while (continueWorking);
                        
                        
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
                else if (menuSelection == send)
                {
                    int userID;
                    decimal amount;

                    List<API_User> otherUsers = accountService.GetAllUsers();
                    
                    Console.WriteLine($"| {id.PadRight(5)} | {name.PadRight(20)}");
                    Console.WriteLine("------------------------------------------------------------");
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
                            foreach(API_User user in otherUsers)
                            {
                                if (user.UserId == userID && amount > 0 && user.UserId != UserService.GetUserId())
                                {
                                    validInput = true;
                                }
                                
                            }
                            if (validInput == false)
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
                    Console.WriteLine($"| {id.PadRight(5)} | {userFrom.PadRight(20)} | {userTo.PadRight(20)} | {sentAmount.PadRight(6)}");
                    Console.WriteLine("------------------------------------------------------------");
                    Console.WriteLine($"| {sendTransfer.TransferId.ToString().PadRight(5)} | {sendTransfer.UserFrom.ToString().PadRight(20)} | {sendTransfer.UserTo.ToString().PadRight(20)} | {sendTransfer.Amount.ToString().PadRight(6)}");
                }
                else if (menuSelection == request)
                {

                    List<API_User> otherUsers = accountService.GetAllUsers();
                    Console.WriteLine($"| {id.PadRight(5)} | {name.PadRight(20)}");
                    Console.WriteLine("------------------------------------------------------------");
                    foreach (API_User user in otherUsers)
                    {
                        Console.WriteLine($"| {user.UserId.ToString().PadRight(5)} | {user.Username.ToString().PadRight(20)}");
                    }

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
                            foreach (API_User user in otherUsers)
                            {
                                if (user.UserId == userID && amount > 0 && user.UserId != UserService.GetUserId())
                                {
                                    validInput = true;
                                }

                                if (validInput == false)
                                {
                                    Console.WriteLine("Please enter a valid User ID and Amount");
                                }
                            }
                        } while (!validInput);

                    }
                    catch (Exception)
                    {

                        throw;
                    }


                    API_Transfer requestTransfer = transferService.RequestTransfer(userID, amount);
                    Console.WriteLine("Transfer Request:");
                    Console.WriteLine($"| {id.PadRight(5)} | {status.PadRight(10)} | {userFrom.PadRight(20)} | {userTo.PadRight(20)} | {sentAmount.PadRight(6)}");
                    Console.WriteLine("------------------------------------------------------------");
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
