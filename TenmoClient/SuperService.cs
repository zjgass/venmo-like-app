using RestSharp;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace TenmoClient
{
    public class SuperService
    {
        protected readonly static string API_BASE_URL = "https://localhost:44315/";
        protected readonly IRestClient client = new RestClient();

        protected bool ProcessResponse(IRestResponse response)
        {
            if ((int)response.StatusCode == 500)
            {
                throw new HttpRequestException($"Error occurred server side.");
            }
            else if ((int)response.StatusCode == 400)
            {
                throw new HttpRequestException("Error occurred - received non-success response: " + response.Content);
            }
            else if ((int)response.StatusCode == 200)
            {
                return true;
            }
            return false;
        }
    }
}
