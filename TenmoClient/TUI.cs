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
                    Console.WriteLine();
                    Console.WriteLine("Your current account balance is: " + currentBalance.Balance);
                }
                else if (menuSelection == pastTransactions)
                {
                    ViewPastTransfers();                    
                }
                else if (menuSelection == pendingRequests)
                {
                    ViewPendingTransfers();
                }
                else if (menuSelection == send)
                {
                    try
                    {
                        API_Transfer transfer = SendRequestTransfer(true);
                        Console.WriteLine(PrintTransferTitle(true));
                        Console.WriteLine(PrintTransfer(transfer, true));
                    }
                    catch (Exception e) { };
                }
                else if (menuSelection == request)
                {

                    try
                    {
                        API_Transfer transfer = SendRequestTransfer(false);
                        Console.WriteLine(PrintTransferTitle(false));
                        Console.WriteLine(PrintTransfer(transfer, false));
                    }
                    catch (Exception e) { };
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

        private void ViewPastTransfers()
        {
            bool continueWorking = true;

            try
            {
                do
                {
                    API_Transfer selectedTransfer = SelectTransfer(true);

                    if (selectedTransfer != null)
                    {
                        string title =
                            "\n--------------------------------------------\n" +
                            "Transfer Details\n" +
                            "--------------------------------------------\n";
                        Console.WriteLine(title);
                        Console.WriteLine(selectedTransfer.ToString());
                    }
                } while (continueWorking);
            }
            catch (Exception e) { }
        }

        private void ViewPendingTransfers()
        {
            bool continueWorking = true;
            try
            {
                do
                {
                    API_Transfer updatedTransfer = SelectTransfer(false);

                    bool correctOption = true;
                    bool leavePending = false;

                    Console.WriteLine("1: Approve\n" +
                                      "2: Reject\n" +
                                      "0: Don't approve or reject");
                    do
                    {
                        correctOption = true;
                        Console.Write("Please choose an option: ");
                        string option = Console.ReadLine().Trim();
                        if (option == "1")
                        {
                            option = "approved";
                            updatedTransfer = transferService.UpdateTransfer(updatedTransfer, option);
                        }
                        else if (option == "2")
                        {
                            option = "rejected";
                            updatedTransfer = transferService.UpdateTransfer(updatedTransfer, option);
                        }
                        else if (option == "0")
                        {
                            leavePending = true;
                        }
                        else
                        {
                            Console.WriteLine("Please select 1, 2, or 0.");
                            correctOption = false;
                        }
                    } while (!correctOption);

                    if (updatedTransfer != null && !leavePending)
                    {
                        Console.WriteLine(PrintTransfer(updatedTransfer, false));
                    }
                } while (continueWorking);
            }
            catch (Exception e) 
            {
                Console.WriteLine(e.Message);
            }
        }

        private API_Transfer SelectTransfer(bool pastTransfers)
        {
            API_Transfer selectedTransfer = new API_Transfer();
            List<API_Transfer> transfers = new List<API_Transfer>();
            if (pastTransfers)
            {
                transfers = transferService.GetPastTransfers();
            }
            else
            {
                transfers = transferService.GetPendingTransers();
            }
            
            if (transfers != null && transfers.Count > 0)
            {
                string titleString = PrintTransferTitle(pastTransfers);
                Console.WriteLine(titleString);

                foreach (API_Transfer transfer in transfers)
                {
                    string txString = PrintTransfer(transfer, pastTransfers);
                    Console.WriteLine(txString);
                }
                Console.Write("---------\n");

                bool correctResponse = false;
                do
                {
                    correctResponse = false;
                    string query = "Please enter transfer ID to ";
                    query += pastTransfers ? "view details(0 to cancel): " : "approve/reject (0 to cancel): ";
                    Console.Write(query);
                    string response = Console.ReadLine().Trim();

                    if (response == "0")
                    {
                        throw new Exception("Done selecting a transfer.");
                    }
                    else
                    {
                        foreach (API_Transfer transfer in transfers)
                        {
                            if (transfer.TransferId.ToString() == response)
                            {
                                selectedTransfer = transfer;
                                correctResponse = true;
                            }
                            else
                            {
                                Console.WriteLine("Please select a valid transfer id.");
                            }
                        }
                    }
                } while (!correctResponse);
            }
            else
            {
                throw new Exception("No transfers to display.");
            }

            return selectedTransfer;
        }

        private string PrintTransferTitle(bool pastTransfer)
        {
            string titleString = "\n-------------------------------------------\n";
            titleString += pastTransfer ? "Transfers\n" : "Pending Transfers\n";
            titleString += "ID          ";
            titleString += pastTransfer ? "From / To             " : "To                    ";
            titleString += "Amount\n";
            titleString += "-------------------------------------------";

            return titleString;
        }

        private string PrintTransfer(API_Transfer transfer, bool pastTransfer)
        {
            string txString = transfer.TransferId.ToString().PadRight(12) +
                        (pastTransfer ? (transfer.UserFromId == UserService.GetUserId() ?
                        "To: " + transfer.UserTo : "From: " + transfer.UserFrom)
                        : transfer.UserTo).PadRight(22) +
                        transfer.Amount.ToString("C2").PadLeft(9);
            return txString;
        }

        private API_Transfer SendRequestTransfer(bool sending)
        {
            API_User selectedUser = SelectUser(sending);

            API_Transfer transfer = new API_Transfer();

            if (sending)
            {
                transfer.TransferType = "Send";
                transfer.TransferStatus = "Approved";
                transfer.UserFromId = UserService.GetUserId();
                transfer.UserToId = selectedUser.UserId;
                transfer.UserTo = selectedUser.Username;
            }
            else
            {
                transfer.TransferType = "Request";
                transfer.TransferStatus = "Pending";
                transfer.UserToId = UserService.GetUserId();
                transfer.UserFromId = selectedUser.UserId;
                transfer.UserFrom = selectedUser.Username;
            }

            decimal amount = 0.01M;
            bool correctResponse = false;
            do
            {
                correctResponse = false;
                Console.Write("Enter amount: ");
                amount = decimal.Parse(Console.ReadLine().Trim());
                if (amount >= 0.01M)
                {
                    correctResponse = true;
                }
                else
                {
                    Console.WriteLine("Please eneter an amount greater than or equal to 0.01.");
                }
            } while (!correctResponse);

            if (sending)
            {
                transfer = transferService.SendTEbucks(selectedUser.UserId, amount);
            }
            else
            {
                transfer = transferService.RequestTransfer(selectedUser.UserId, amount);
            }

            return transfer;
        }

        private API_User SelectUser(bool sending)
        {
            API_User selectedUser = new API_User();
            List<API_User> otherUsers = accountService.GetAllUsers();

            if (otherUsers != null && otherUsers.Count > 0)
            {
                Console.WriteLine();
                Console.WriteLine(
                    "-------------------------------------------\n" +
                    "Users\n" +
                    "ID          Name\n" +
                    "-------------------------------------------");
                foreach (API_User user in otherUsers)
                {
                    string userString = user.UserId.ToString().PadRight(12) +
                        user.Username.ToString();
                    Console.WriteLine(userString);
                }
                Console.Write("---------\n\n");

                bool correctResponse = false;
                do
                {
                    correctResponse = false;
                    string query = "Enter ID of user you are ";
                    query += sending ? "sending to " : "requesting from ";
                    query += "(0 to cancel): ";
                    Console.Write(query);
                    string response = Console.ReadLine().Trim();

                    if (response == "0")
                    {
                        throw new Exception("Done selecting a user.");
                    }
                    else
                    {
                        foreach (API_User user in otherUsers)
                        {
                            if (user.UserId.ToString() == response)
                            {
                                selectedUser = user;
                                correctResponse = true;
                            }
                            else
                            {
                                Console.WriteLine("Please select a valid user id.");
                            }
                        }
                    }
                } while (!correctResponse);
            }

            return selectedUser;
        }
    }
}
