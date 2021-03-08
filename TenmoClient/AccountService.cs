using System;
using System.Collections.Generic;
using System.Text;
using RestSharp;
using RestSharp.Authenticators;
using TenmoClient.Data;

namespace TenmoClient
{
    public class AccountService : SuperService
    {
        public API_Account GetAccountBalance()
        {
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());

            RestRequest request = new RestRequest(API_BASE_URL + "api/account");
            IRestResponse<API_Account> balance = client.Get<API_Account>(request);
            if (ProcessResponse(balance))
            {
                return balance.Data;
            }
            return null;
        }

        public List<API_User> GetAllUsers()
        {
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());

            RestRequest request = new RestRequest(API_BASE_URL + "api/user");
            IRestResponse<List<API_User>> allUsers = client.Get<List<API_User>>(request);
            if (ProcessResponse(allUsers))
            {
                return allUsers.Data;
            }
            return null;
        }
    }
}
