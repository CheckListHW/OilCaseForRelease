using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Trait("Category", "Integration")]
public abstract class IntegrationTest
{
    protected readonly HttpClient _client;

    protected string? _token;

    public IntegrationTest()
    {
        var server = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder => { });
        _client = server.CreateClient();
        new HttpRequestMessage(new HttpMethod("GET"), "");

        _token = GetToken("p1@p1.com", "111111");
        File.WriteAllText("C:\\Users\\kosac\\source\\repos\\OilCaseX\\dev\\OilCaseX.tests\\Tests\\Modules\\text.txt", _token);
    }

    public string? GetToken(string username, string password)
    {
        var login = new OilCaseApi.Controllers.ApiModels.Login()
        {
            Username = username,
            Password = password
        };

        var request = new HttpRequestMessage(new HttpMethod("POST"), "/api/v1/Login");
        
        request.Content = new StringContent(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");

        var response = _client.SendAsync(request);

        return response.Result.Content.ReadAsStringAsync().Result;
    }
}
