using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using OilCaseApi.Controllers.ApiModels;

namespace OilCaseX.tests.Tests.Controllers.LithologicalData
{
    public class ObjectOfArrangement
    {
        private readonly HttpClient _client;

        public ObjectOfArrangement()
        {
            var server = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {

                });

            _client = server.CreateClient();
        }
    }
}
