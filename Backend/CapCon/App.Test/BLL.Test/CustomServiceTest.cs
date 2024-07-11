using App.BLL;
using App.BLL.Services;
using App.DAL.EF;
using App.DAL.EF.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace App.Test.BLL.Test;

public class CustomServiceTest
{
    private readonly AppDbContext _ctx;
    private readonly CompanyRepository _companyRepository;
    private readonly CompanyService _companyService;
    private readonly AppUOW _uow;
    
    public CustomServiceTest()
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
            
            cfg.CreateMap<App.BLL.DTO.Company, App.DAL.DTO.Company>().ReverseMap();
            cfg.CreateMap<App.BLL.DTO.UserCompany, App.DAL.DTO.UserCompany>().ReverseMap();
            cfg.CreateMap<App.BLL.DTO.Identity.AppUser, App.DAL.DTO.Identity.AppUser>().ReverseMap();
            cfg.CreateMap<App.BLL.DTO.Loan, App.DAL.DTO.Loan>().ReverseMap();
            cfg.CreateMap<App.BLL.DTO.Transaction, App.DAL.DTO.Transaction>().ReverseMap();
            cfg.CreateMap<App.BLL.DTO.LoanRequest, App.DAL.DTO.LoanRequest>().ReverseMap();

        });
        var mapper = config.CreateMapper();
        _companyRepository =
            new CompanyRepository(
                _ctx,
                mapper
            );
        
        _uow = new AppUOW(_ctx, mapper);
        _companyService = new CompanyService(_uow, _companyRepository, mapper);
    }
    
    public async Task<List<Guid>> SeedData(int numberOfCompanies)
    {
        var user = new Domain.Identity.AppUser()
        {
            FirstName = "Test",
            LastName = "Bar",
            Email = "a@b.c"
        };
        _uow.Users.Add(user);
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
        
        _uow.Users.Add(user);
        await _uow.SaveChangesAsync();
                
        var c = new App.BLL.DTO.Company(){ CompanyName = "Foo" };

        // Act
        c = await _companyService.AddCompanyWithManagerAsync(c, user.Id);
        await _uow.SaveChangesAsync();

        // Assert
        var company = await _companyRepository.FirstOrDefaultCompanyIncludingAsync(c.Id);
        Assert.NotNull(company);
        Assert.NotNull(company.UserCompanies);
        Assert.Equal(1, company.UserCompanies.Count());
        Assert.NotNull(company.UserCompanies.First());
        var uc = company.UserCompanies.First();
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
        _uow.Users.Add(user);
        await _uow.SaveChangesAsync();
                
        var c1 = new App.BLL.DTO.Company(){ CompanyName = "A" };
        var c2 = new App.BLL.DTO.Company(){ CompanyName = "B" };

        // Act
        var company1 = await _companyService.AddCompanyWithManagerAsync(c1, user.Id);
        var company2 = await _companyService.AddCompanyWithManagerAsync(c2, user.Id);
        await _uow.SaveChangesAsync();
        var companies = await _companyService.GetAllSortedAsync(user.Id);

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
        var companies = await _companyService.GetAllCompaniesIncludingAsync();
        
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
        var company = await _companyService.FirstOrDefaultCompanyIncludingAsync(id);
        
        // Assert
        Assert.NotNull(company);
        Assert.NotNull(company.UserCompanies);
        Assert.Equal(id, company.UserCompanies.First().CompanyId);
        Assert.NotNull(company.Loans);
        Assert.Equal(id, company.Loans.First().CompanyId);
        Assert.NotNull(company.Loans.First().Transactions);
        Assert.NotNull(company.LoanRequests);
        Assert.Equal(id, company.LoanRequests.First().CompanyId);

        
    }
}