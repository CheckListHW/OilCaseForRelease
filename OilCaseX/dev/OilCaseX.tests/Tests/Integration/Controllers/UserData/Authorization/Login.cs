using OilCaseApi.Controllers.ApiModels;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace OilCaseX.tests.Tests.Controllers.UserData.Authorization
{
    public class Login : IntegrationTest
    {
        public Login() : base() { }

        /// <summary>
        /// ������������� ��� 200 ��� ���������� ������ � ������
        /// </summary>
        [Theory]
        [InlineData("POST")]
        public async void CheckCorrectAuth(string method)
        {
            //Arrange
            var request = new HttpRequestMessage(new HttpMethod(method), "/api/v1/Login");
            var login = new OilCaseApi.Controllers.ApiModels.Login()
            {
                Username = "p1@p1.com",
                Password = "111111"
            };
            request.Content = new StringContent(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");

            //Act
            var response = await _client.SendAsync(request);

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // ����� ���� ����� ���������� ���������
            File.WriteAllText("C:\\Users\\kosac\\Desktop\\text.txt", response.Content.ReadAsStreamAsync().ToString());
        }


        /// <summary>
        /// ������������� ��� 200 ��� �������� ������ � ������
        /// </summary>
        [Theory]
        [InlineData("POST")]
        public async void CheckUnCorrectAuth(string method)
        {
            //Arrange
            var request = new HttpRequestMessage(new HttpMethod(method), "/api/v1/Login");
            var login = new OilCaseApi.Controllers.ApiModels.Login()
            {
                Username = "1",
                Password = "1"
            };
            request.Content = new StringContent(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");

            //Act
            var response = await _client.SendAsync(request);

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}