using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
//using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace TenmoServerTests.TransferControllerTests
{
    [TestClass]
    public class GetAllCompletedTransfersTests
    {
        protected HttpClient _client;

        [TestInitialize]
        public void Setup()
        {
            var builder = new WebHostBuilder().UseStartup<TenmoServer.Startup>();
            //var server = new TestServer(builder);
            //_client = server.CreateClient();
        }
    }
}
