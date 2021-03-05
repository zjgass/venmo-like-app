using System;
using System.Collections.Generic;
using System.Text;
using RestSharp;
using RestSharp.Authenticators;
using TenmoClient.Data;

namespace TenmoClient
{
    public class TransferService
    {
        private readonly static string API_BASE_URL = "https://localhost:44315/";
        private readonly IRestClient client = new RestClient();

        public List<API_Transfer> GetPastTransfers()
        {
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());

            RestRequest request = new RestRequest(API_BASE_URL + "api/transfer");
            IRestResponse<List<API_Transfer>> allTransfers = client.Get<List<API_Transfer>>(request);
            return allTransfers.Data;
        }

        public List<API_Transfer> GetPendingTransers()
        {
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());

            RestRequest request = new RestRequest(API_BASE_URL + "api/transfer/pending");
            IRestResponse<List<API_Transfer>> allPendingTransfers = client.Get<List<API_Transfer>>(request);
            return allPendingTransfers.Data;
        }

        public API_Transfer SendTEbucks()
        {
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());

            RestRequest request = new RestRequest(API_BASE_URL + "api/transfer");
            IRestResponse<API_Transfer> sendTransfer = client.Post<API_Transfer>(request);
            return sendTransfer.Data;
        }
    }
}
