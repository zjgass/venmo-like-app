using System;
using System.Collections.Generic;
using System.Text;
using RestSharp;
using RestSharp.Authenticators;
using TenmoClient.Data;

namespace TenmoClient
{
    public class TransferService : SuperService
    {
        public List<API_Transfer> GetPastTransfers()
        {
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());

            RestRequest request = new RestRequest(API_BASE_URL + "api/transfer");
            IRestResponse<List<API_Transfer>> allTransfers = client.Get<List<API_Transfer>>(request);
            if (ProcessResponse(allTransfers))
            {
                return allTransfers.Data;
            }
            return null;
        }

        public List<API_Transfer> GetPendingTransers()
        {
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());

            RestRequest request = new RestRequest(API_BASE_URL + "api/transfer/pending");
            IRestResponse<List<API_Transfer>> allPendingTransfers = client.Get<List<API_Transfer>>(request);
            if (ProcessResponse(allPendingTransfers))
            {
                return allPendingTransfers.Data;
            }
            return null;
        }

        public API_Transfer SendTEbucks(int userID, decimal amount)
        {
            //set up a transfer object
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());

            API_Transfer newTransfer = new API_Transfer();
            newTransfer.TransferType = "Send";
            newTransfer.TransferStatus = "Approved";
            newTransfer.UserFrom = "";
            newTransfer.UserFromId = UserService.GetUserId();
            newTransfer.UserTo = "";
            newTransfer.UserToId = userID;
            newTransfer.Amount = amount;

        //add that to the jsonbody

            RestRequest request = new RestRequest(API_BASE_URL + "api/transfer");
            request.AddJsonBody(newTransfer);
            IRestResponse<API_Transfer> sendTransfer = client.Post<API_Transfer>(request);
            if (ProcessResponse(sendTransfer))
            {
                return sendTransfer.Data;
            }
            return null;
        }

        public API_Transfer RequestTransfer(int userID, decimal amount)
        {
            //set up a transfer object
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());

            API_Transfer requestTransfer = new API_Transfer();
            requestTransfer.TransferType = "Request";
            requestTransfer.TransferStatus = "Pending";
            requestTransfer.UserFrom = "";
            requestTransfer.UserFromId = userID;
            requestTransfer.UserTo = "";
            requestTransfer.UserToId = UserService.GetUserId();
            requestTransfer.Amount = amount;

            //add that to the jsonbody

            RestRequest request = new RestRequest(API_BASE_URL + "api/transfer/request");
            request.AddJsonBody(requestTransfer);
            IRestResponse<API_Transfer> sendTransfer = client.Post<API_Transfer>(request);
            if (ProcessResponse(sendTransfer))
            {
                return sendTransfer.Data;
            }
            return null;
        }

        public API_Transfer UpdateTransfer(API_Transfer transfer, string option)
        {
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());

            RestRequest request = new RestRequest(API_BASE_URL + $"api/transfer/{transfer.TransferId}");
            transfer.TransferStatus = option;
            request.AddJsonBody(transfer);
            IRestResponse<API_Transfer> updateTransfer = client.Put<API_Transfer>(request);
            if (ProcessResponse(updateTransfer))
            {
                return updateTransfer.Data;
            }
            return null;
        }

        public API_Transfer GetTransfer(int transferId)
        {
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());

            RestRequest request = new RestRequest(API_BASE_URL + $"api/transfer/{transferId}");
            IRestResponse<API_Transfer> transfer = client.Get<API_Transfer>(request);

            if (ProcessResponse(transfer))
            {
                return transfer.Data;
            }
            return null;
        }
    }
}
