using System;
using System.Collections.Generic;
using System.Text;
using RestSharp;
using TenmoClient.Data;

namespace TenmoClient
{
    public class AccountService
    {
        private readonly static string API_BASE_URL = "https://localhost:44315/";
        private readonly IRestClient client = new RestClient();

        public API_Account GetAccountBalance()
        {
            RestRequest request = new RestRequest(API_BASE_URL + "api/account");
            IRestResponse<API_Account> balance = client.Get<API_Account>(request);
            return balance.Data;
        }

    }
}
