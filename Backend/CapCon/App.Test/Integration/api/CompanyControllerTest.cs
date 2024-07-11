using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using App.DTO.v1_0.Identity;
using Helpers;
using Microsoft.AspNetCore.Mvc.Testing;

namespace App.Test.Integration.api;

public class CompanyControllerTest : IClassFixture<CustomWebApplicationFactory<Program>>
{
private readonly HttpClient _client;
private readonly CustomWebApplicationFactory<Program> _factory;
    
    public CompanyControllerTest(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }
        
    [Fact]
    public async Task IndexRequiresLogin()
    {
        // Arrange

        // Act
        var response = await _client.GetAsync("/api/v1.0/Companies");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact]
    public async Task IndexWithUser()
    {
        var user = "admin@capcon.com";
        var pass = "Hello123!";

        var response = await _client.PostAsJsonAsync("/api/v1.0/identity/Account/Login", new {email = user, password = pass});
        var contentStr = await response.Content.ReadAsStringAsync();
        var loginData = System.Text.Json.JsonSerializer.Deserialize<JWTResponse>(contentStr, JsonHelper.CamelCase);
        Assert.NotNull(loginData);
        Assert.NotEmpty(loginData.Jwt);
        
        var msg = new HttpRequestMessage(HttpMethod.Get, "/api/v1.0/Companies");
        msg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", loginData.Jwt);
        msg.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        response = await  _client.SendAsync(msg);
        response.EnsureSuccessStatusCode();
    }
}