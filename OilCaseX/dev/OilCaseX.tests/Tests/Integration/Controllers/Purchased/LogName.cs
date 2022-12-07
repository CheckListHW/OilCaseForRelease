using OilCaseApi.Controllers.ApiModels;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using OilCaseApi.Models;
using Microsoft.EntityFrameworkCore;
using OilCaseApi.Controllers.Api;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace OilCaseX.tests.Tests.Integration.Controllers.Purchased
{
    public class LogName : IntegrationTest
    {
        public LogName() : base() { }

        [Theory]
        [InlineData("GET")]
        public async void CheckGETUnAuthorized(string method)
        {
            //Arrange
            string token = "";
            var request = new HttpRequestMessage(new HttpMethod(method), "/api/v1/Purchased/LogName");
            request.Headers.Add("Authorization", $"Bearer {token}");

            //Act
            var response = await _client.SendAsync(request);

            //Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory]
        [InlineData("GET")]
        public async void CheckGETAuthorized(string method)
        {
            //Arrange
            var request = new HttpRequestMessage(new HttpMethod(method), "/api/v1/Purchased/LogName");
            request.Headers.Add("Authorization", $"Bearer {_token}");

            //Act
            var response = await _client.SendAsync(request);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
