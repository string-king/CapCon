using App.DAL.EF;
using App.DAL.EF.Repositories;
using App.Domain;
using App.DAL.DTO;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace App.Test.DAL.Test;

public class CustomRepositoryTest
{
    private readonly AppDbContext _ctx;
    private readonly CompanyRepository _companyRepository;

    public CustomRepositoryTest()
    {
        // set up mock database - inmemory
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        // use random guid as db instance id
        optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
        _ctx = new AppDbContext(optionsBuilder.Options);

        // reset db
        _ctx.Database.EnsureDeleted();
        _ctx.Database.EnsureCreated();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<App.Domain.Company, App.DAL.DTO.Company>().ReverseMap();
            cfg.CreateMap<App.Domain.UserCompany, App.DAL.DTO.UserCompany>().ReverseMap();
            cfg.CreateMap<App.Domain.Identity.AppUser, App.DAL.DTO.Identity.AppUser>().ReverseMap();
            cfg.CreateMap<App.Domain.Loan, App.DAL.DTO.Loan>().ReverseMap();
            cfg.CreateMap<App.Domain.Transaction, App.DAL.DTO.Transaction>().ReverseMap();
            cfg.CreateMap<App.Domain.LoanRequest, App.DAL.DTO.LoanRequest>().ReverseMap();

        });
        var mapper = config.CreateMapper();
        _companyRepository =
            new CompanyRepository(
                _ctx,
                mapper
            );
    }
    
    public async Task<List<Guid>> SeedData(int numberOfCompanies)
    {
        var user = new Domain.Identity.AppUser()
        {
            FirstName = "Test",
            LastName = "Bar",
            Email = "a@b.c"
        };
        _ctx.Users.Add(user);
        await _ctx.SaveChangesAsync();

        var companyIds = new List<Guid>();

        for (int i = 0; i < numberOfCompanies; i++)
        {
            var companyDto = new App.DAL.DTO.Company() { CompanyName = $"Company {i + 1}" };
            var company = await _companyRepository.AddCompanyWithManagerAsync(companyDto, user.Id);
            await _ctx.SaveChangesAsync();

            var loan = new App.Domain.Loan() { CompanyId = company.Id, AppUserId = user.Id };
            _ctx.Loans.Add(loan);

            var transaction = new App.Domain.Transaction() { LoanId = loan.Id };
            _ctx.Transactions.Add(transaction);

            var loanRequest = new App.Domain.LoanRequest() { CompanyId = company.Id };
            _ctx.LoanRequests.Add(loanRequest);

            companyIds.Add(company.Id);
        }

        await _ctx.SaveChangesAsync();

        return companyIds;
    }
    
    [Fact]
    public async Task TestAddCompanyWithManagerAsync()
    {
        // Arrange
        
        var user = new Domain.Identity.AppUser()
        {
            FirstName = "Test",
            LastName = "Bar",
            Email = "a@b.c"
        };
        
        _ctx.Users.Add(user);
        await _ctx.SaveChangesAsync();
                
        var c = new App.DAL.DTO.Company(){ CompanyName = "Foo" };

        // Act
        var company = await _companyRepository.AddCompanyWithManagerAsync(c, user.Id);
        await _ctx.SaveChangesAsync();

        // Assert
        var uc = company.UserCompanies.First();
        Assert.NotNull(uc);
        Assert.Equal(user.Id, uc.AppUserId);
        Assert.Equal(App.DAL.DTO.Enums.ECompanyRole.Manager, uc.Role);
    }
    
    [Fact]
    public async Task TestGetAllSortedAsync()
    {
        // Arrange
        
        var user = new Domain.Identity.AppUser()
        {
            FirstName = "Test",
            LastName = "Bar",
            Email = "a@b.c"
        };
        _ctx.Users.Add(user);
        _ctx.SaveChangesAsync();
                
        var c1 = new App.DAL.DTO.Company(){ CompanyName = "A" };
        var c2 = new App.DAL.DTO.Company(){ CompanyName = "B" };

        // Act
        var company1 = await _companyRepository.AddCompanyWithManagerAsync(c1, user.Id);
        var company2 = await _companyRepository.AddCompanyWithManagerAsync(c2, user.Id);
        await _ctx.SaveChangesAsync();
        var companies = await _companyRepository.GetAllSortedAsync(user.Id);

        // Assert
        Assert.Equal(companies.Count(), 2);
        Assert.Equal("A", companies.First().CompanyName);
        Assert.NotNull(companies.First().UserCompanies);
        
    }
    
    [Fact]
    public async Task TestGetAllIncludingAsync()
    {
        // Arrange
        
        await SeedData(2);
        
        // Act
        var companies = await _companyRepository.GetAllCompaniesIncludingAsync();
        
        // Assert
        Assert.Equal(companies.Count(), 2);
        Assert.NotNull(companies.First().UserCompanies);
        Assert.NotNull(companies.First().Loans);
        Assert.NotNull(companies.First().Loans.First().Transactions);
        Assert.NotNull(companies.First().LoanRequests);
        
    }
    
    [Fact]
    public async Task TestGetFirstIncludingAsync()
    {
        // Arrange
        
        var id = (await SeedData(1)).First();
        
        // Act
        var company = await _companyRepository.FirstOrDefaultCompanyIncludingAsync(id);
        
        // Assert
        Assert.NotNull(company);
        Assert.NotNull(company.UserCompanies);
        Assert.NotNull(company.Loans);
        Assert.NotNull(company.Loans.First().Transactions);
        Assert.NotNull(company.LoanRequests);
        
    }
    
    
}