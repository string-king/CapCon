using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using App.DTO.v1_0;
using App.DTO.v1_0.Enums;
using App.DTO.v1_0.Identity;
using Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Loan = App.BLL.DTO.Loan;

namespace App.Test.Integration;

public class AppIntegrationTest : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;
    private readonly ILogger<AppIntegrationTest> _logger;
    
    

    
    public AppIntegrationTest(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
        _logger = _factory.Services.GetRequiredService<ILogger<AppIntegrationTest>>();
    }

    [Fact]
    public async Task LoanFlow()
    {
        await RegisterBorrower();
        await RegisterLendor();
        await CreateCompanyAndLoanRequest();
        await CreateLoanOffer();
        await AcceptLoanOffer();
        await LendorPaysInitialAmount();
        await BorrowerPaysLoanBalance();
        await LoanNotActive();
    }
    
    public async Task<JWTResponse> Login(string email, string password)
    {
        var response = await _client.PostAsJsonAsync("/api/v1/identity/Account/Login", new {email, password});
        var contentStr = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();
        
        var loginData = JsonSerializer.Deserialize<JWTResponse>(contentStr, JsonHelper.CamelCase);
        Assert.NotNull(loginData);
        Assert.NotNull(loginData.Jwt);
        Assert.True(loginData.Jwt.Length > 0);
        
        return loginData;
    }

    public async Task Logout(JWTResponse loginData)
    {
        var msg = new HttpRequestMessage(HttpMethod.Post, "/api/v1/identity/Account/Logout");
        msg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", loginData.Jwt);
        msg.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        
        var response = await _client.SendAsync(msg);
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task RegisterBorrower()
    {
        var registerResponse = await _client.PostAsJsonAsync("/api/v1/identity/Account/Register",
            new { email = "borrower@capcon.com", password = "FooBar123!", firstname = "Borro", lastname = "Wer" });
        registerResponse.EnsureSuccessStatusCode();
        var registerContent = await registerResponse.Content.ReadAsStringAsync();
        var loginData = JsonSerializer.Deserialize<JWTResponse>(registerContent, JsonHelper.CamelCase);
        Assert.NotNull(loginData);
        Assert.NotNull(loginData.Jwt);
        Assert.True(loginData.Jwt.Length > 0);
        
        Logout(loginData);
    }
    
    [Fact]
    public async Task RegisterLendor()
    {
        // Register the lender and get the JWT token
        var registerResponse = await _client.PostAsJsonAsync("/api/v1/identity/Account/Register", new
        {
            email = "lendor@capcon.com",
            password = "FooBar123!",
            firstname = "Lenn",
            lastname = "Dorr"
        });
        registerResponse.EnsureSuccessStatusCode();
        var registerContent = await registerResponse.Content.ReadAsStringAsync();
        var loginData = JsonSerializer.Deserialize<JWTResponse>(registerContent, JsonHelper.CamelCase);
        Assert.NotNull(loginData);
        Assert.NotNull(loginData.Jwt);
        Assert.True(loginData.Jwt.Length > 0);
        
        Logout(loginData);
    }
    
    [Fact]
    public async Task CreateCompanyAndLoanRequest()
    {
        var loginData = await Login("borrower@capcon.com", "FooBar123!");
        var jwt = loginData.Jwt;

        var companyDto = new App.DTO.v1_0.Company() { Id = Guid.NewGuid(), CompanyName = "Foo" };
        var createCompanyRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1.0/Companies")
        {
            Content = JsonContent.Create(new { id = companyDto.Id, companyName = companyDto.CompanyName })
        };
        createCompanyRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        var createCompanyResponse = await _client.SendAsync(createCompanyRequest);
        createCompanyResponse.EnsureSuccessStatusCode();
        var companyContent = await createCompanyResponse.Content.ReadAsStringAsync();
        var company = JsonSerializer.Deserialize<Company>(companyContent, JsonHelper.CamelCase);
        Assert.NotNull(company);
        Assert.Equal(company.CompanyName, companyDto.CompanyName);

        var lr = new LoanRequestSimple()
        {
            CompanyId = company.Id,
            Amount = 1000,
            Interest = 5.0m,
            Period = 12,
            Active = true,
            Comment = "FooBar"
        };

        var createLoanRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1.0/LoanRequests")
        {
            Content = JsonContent.Create(lr)
        };
        
        createLoanRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        var createLoanRequestResponse = await _client.SendAsync(createLoanRequest);
        createLoanRequestResponse.EnsureSuccessStatusCode();
        var loanRequestContent = await createLoanRequestResponse.Content.ReadAsStringAsync();
        var loanRequest = JsonSerializer.Deserialize<LoanRequestSimple>(loanRequestContent, JsonHelper.CamelCase);
        Assert.NotNull(loanRequest);
        Assert.Equal(lr.CompanyId, loanRequest.CompanyId);
        Assert.Equal(lr.Amount, loanRequest.Amount);
        Assert.Equal(lr.Interest, loanRequest.Interest);
        Assert.Equal(lr.Period, loanRequest.Period);
        Assert.Equal(lr.Active, loanRequest.Active);
        Assert.Equal(lr.Comment, loanRequest.Comment);
        
        Logout(loginData);
    }

    
    [Fact]
    public async Task CreateLoanOffer()
    {
        var loginData = await Login("lendor@capcon.com", "FooBar123!");
        var jwt = loginData.Jwt;
        
        // Get the list of companies
        var getCompaniesRequest = new HttpRequestMessage(HttpMethod.Get, "/api/v1.0/Companies");
        getCompaniesRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

        var getCompaniesResponse = await _client.SendAsync(getCompaniesRequest);
        getCompaniesResponse.EnsureSuccessStatusCode();
        var companiesContent = await getCompaniesResponse.Content.ReadAsStringAsync();
        var companies = JsonSerializer.Deserialize<List<Company>>(companiesContent, JsonHelper.CamelCase);
        Assert.NotNull(companies);
        Assert.True(companies.Count == 1);
        
        var company = companies[0];
        Assert.True(company.LoanRequests!.Count == 1);
        var loanRequest = company.LoanRequests.First();
        
        var lo = new LoanOffer()
        {
            LoanRequestId = loanRequest.Id,
            Amount = 750,
            Interest = Decimal.Parse("10", CultureInfo.InvariantCulture),
            Period = 10,
            Active = true,
            Comment = "FooBar"
        };
        
        var createLoanOfferRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1.0/LoanOffers")
        {
            Content = JsonContent.Create(lo)
        };
        createLoanOfferRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
        createLoanOfferRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var createLoanOfferResponse = await _client.SendAsync(createLoanOfferRequest); 
        
        await createLoanOfferResponse.Content.ReadAsStringAsync();
        createLoanOfferResponse.EnsureSuccessStatusCode();
        var loanOfferContent = await createLoanOfferResponse.Content.ReadAsStringAsync();
        var loanOffer = JsonSerializer.Deserialize<LoanOffer>(loanOfferContent, JsonHelper.CamelCase);
        Assert.NotNull(loanOffer);
        Assert.Equal(lo.LoanRequestId, loanOffer.LoanRequestId);
        Assert.Equal(lo.Amount, loanOffer.Amount);
        Assert.Equal(lo.Interest, loanOffer.Interest);
        Assert.Equal(lo.Period, loanOffer.Period);
        Assert.True(loanOffer.Active);
        
        Logout(loginData);
    }


    [Fact]
    public async Task AcceptLoanOffer()
    {
        var loginData = await Login("borrower@capcon.com", "FooBar123!");
        
        
        var getLoanOffersRequest = new HttpRequestMessage(HttpMethod.Get, "/api/v1.0/LoanOffers");
        getLoanOffersRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", loginData.Jwt);
        
        var getLoanOffersResponse = await _client.SendAsync(getLoanOffersRequest);
        getLoanOffersResponse.EnsureSuccessStatusCode();
        var loanOffersContent = await getLoanOffersResponse.Content.ReadAsStringAsync();
        var loanOffers = JsonSerializer.Deserialize<List<LoanOffer>>(loanOffersContent, JsonHelper.CamelCase);
        Assert.NotNull(loanOffers);
        Assert.True(loanOffers.Count == 1);
        
        var loanOffer = loanOffers[0];
        Assert.NotNull(loanOffer.LoanRequestId);
        Assert.NotNull(loanOffer.AppUserId);
        
        
        var lo = new LoanOffer()
        {
            AppUserId = loanOffer.AppUserId,
            LoanRequestId = loanOffer.Id,
            Amount = 750,
            Interest = Decimal.Parse("10", CultureInfo.InvariantCulture),
            Period = 10,
            Active = true,
            Comment = "FooBar",
            CreatedAt = loanOffer.CreatedAt
        };
        
        var loanOfferAcceptRequest
            = new HttpRequestMessage(HttpMethod.Post, "/api/v1.0/LoanOffers/accept")
        {
            Content = JsonContent.Create(lo)
        };
        loanOfferAcceptRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", loginData.Jwt);
        loanOfferAcceptRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var loanOfferAcceptResponse = await _client.SendAsync(loanOfferAcceptRequest); 
        
        await loanOfferAcceptResponse.Content.ReadAsStringAsync();
        loanOfferAcceptResponse.EnsureSuccessStatusCode();
        
        var loanContent = await loanOfferAcceptResponse.Content.ReadAsStringAsync();
        var loan = JsonSerializer.Deserialize<App.DAL.DTO.Loan>(loanContent, JsonHelper.CamelCase);
        
        Assert.NotNull(loan);
        Assert.Equal(lo.Amount, loan.Amount);
        Assert.Equal(lo.Interest, loan.Interest);
        Assert.Equal(lo.Period, loan.Period);
        Assert.False(loan.Active);
        
        Logout(loginData);
    }

    
    
    

    [Fact]
    public async Task LendorPaysInitialAmount()
    {
        var loginData = await Login("lendor@capcon.com", "FooBar123!");
        
        var msg = new HttpRequestMessage(HttpMethod.Get, "/api/v1/Loans");
        msg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", loginData.Jwt);
        msg.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        
        var response = await _client.SendAsync(msg);
        var contentStr = await response.Content.ReadAsStringAsync();
        
        response.EnsureSuccessStatusCode();
        
        var loans = JsonSerializer.Deserialize<List<LoanOffer>>(contentStr, JsonHelper.CamelCase);
        Assert.NotNull(loans);
        Assert.True(loans.Count == 1);
        
        var loanId = loans[0].Id.ToString();
        
        var transaction = new Transaction()
        {
           LoanId = Guid.Parse(loanId),
           TransactionType = ETransactionType.LenderToBorrower,
           Amount = 750,
           Comment = "FooBar"
        };
        var content = JsonContent.Create(transaction);

        msg = new HttpRequestMessage(HttpMethod.Post, "/api/v1.0/Transactions");
        msg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", loginData.Jwt);
        msg.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        msg.Content = content;

         response = await _client.SendAsync(msg);
         contentStr = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();

        var t = JsonSerializer.Deserialize<Transaction>(contentStr, JsonHelper.CamelCase);
        Assert.NotNull(t);
        Assert.Equal(transaction.LoanId, t.LoanId);
        Assert.Equal(transaction.Amount, t.Amount);
        Assert.Equal(transaction.Comment, t.Comment);
    }
    
    [Fact]
    public async Task BorrowerPaysLoanBalance()
    {
        var loginData = await Login("borrower@capcon.com", "FooBar123!");
        
        var msg = new HttpRequestMessage(HttpMethod.Get, "/api/v1/Loans");
        msg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", loginData.Jwt);
        msg.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        
        var response = await _client.SendAsync(msg);
        response.EnsureSuccessStatusCode();
        var contentStr = await response.Content.ReadAsStringAsync();
        
        var loans = JsonSerializer.Deserialize<List<LoanOffer>>(contentStr, JsonHelper.CamelCase);
        Assert.NotNull(loans);
        Assert.True(loans.Count == 1);
        
        var transaction = new Transaction()
        {
            LoanId = Guid.Parse(loans[0].Id.ToString()),
            TransactionType = ETransactionType.BorrowerToLender,
            Amount = 825,
            Comment = "FooBar"
        };
        var content = JsonContent.Create(transaction);

        msg = new HttpRequestMessage(HttpMethod.Post, "/api/v1.0/Transactions");
        msg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", loginData.Jwt);
        msg.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        msg.Content = content;

        response = await _client.SendAsync(msg);
        contentStr = await response.Content.ReadAsStringAsync();
        response.EnsureSuccessStatusCode();

        var t = JsonSerializer.Deserialize<Transaction>(contentStr, JsonHelper.CamelCase);
        Assert.NotNull(t);
        Assert.Equal(transaction.LoanId, t.LoanId);
        Assert.Equal(transaction.Amount, t.Amount);
        Assert.Equal(transaction.Comment, t.Comment);
    }
    
    [Fact]
    public async Task LoanNotActive()
    {
        var loginData = await Login("lendor@capcon.com", "FooBar123!");
        
        var msg = new HttpRequestMessage(HttpMethod.Get, "/api/v1/Loans");
        msg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", loginData.Jwt);
        msg.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        
        var response = await _client.SendAsync(msg);
        response.EnsureSuccessStatusCode();
        var contentStr = await response.Content.ReadAsStringAsync();
        
        var loans = JsonSerializer.Deserialize<List<LoanOffer>>(contentStr, JsonHelper.CamelCase);
        Assert.NotNull(loans);
        Assert.True(loans.Count == 1);
        Assert.False(loans[0].Active);
    }
}