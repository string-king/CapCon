using App.BLL;
using App.Contracts.BLL;
using App.Contracts.DAL;
using App.DAL.DTO;
using App.DAL.EF;
using App.Domain.Identity;
using AutoMapper;
using Base.Test.DAL;
using Base.Test.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using WebApp.ApiControllers;

namespace App.Test;

public class ApiControllerCompaniesTest
{
    private AppDbContext _ctx;
    private IAppBLL _bll;
    private  IAppUnitOfWork _uow;

    private UserManager<AppUser> _userManager;

    // sut
    private CompaniesController _controller;
    
    public ApiControllerCompaniesTest()
    {
        // set up mock database - inmemory
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        // use random guid as db instance id
        optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
        _ctx = new AppDbContext(optionsBuilder.Options);

        // reset db
        _ctx.Database.EnsureDeleted();
        _ctx.Database.EnsureCreated();
        
        var configUow = new MapperConfiguration(cfg => cfg.CreateMap<App.DAL.DTO.Company, App.BLL.DTO.Company>().ReverseMap());
        var mapperUow = configUow.CreateMapper();
        
        var configBll = new MapperConfiguration(cfg => cfg.CreateMap<App.Domain.Company, App.DAL.DTO.Company>().ReverseMap());
        var mapperBll = configBll.CreateMapper();
        
        var confiWeb = new MapperConfiguration(cfg => cfg.CreateMap<App.DAL.DTO.Company, DTO.v1_0.Company>().ReverseMap());
        var mapperWeb = confiWeb.CreateMapper();
        
        _uow = new AppUOW(_ctx, mapperUow);
        
        _bll = new AppBLL(_uow, mapperBll);
        
        var storeStub = Substitute.For<IUserStore<AppUser>>();
        var optionsStub = Substitute.For<IOptions<IdentityOptions>>();
        var hasherStub = Substitute.For<IPasswordHasher<AppUser>>();
        var userValidatorStub = Substitute.For<IEnumerable<IUserValidator<AppUser>>>();
        var passwordValidatorStub = Substitute.For<IEnumerable<IPasswordValidator<AppUser>>>();
        var lookupNormalizerStub = Substitute.For<ILookupNormalizer>();
        var describerStub = Substitute.For<IdentityErrorDescriber>();
        var serviceProviderStub = Substitute.For<IServiceProvider>();
        var loggerStub = Substitute.For<ILogger<UserManager<AppUser>>>();

        _userManager = Substitute.For<UserManager<AppUser>>(storeStub, optionsStub, hasherStub, userValidatorStub, 
            passwordValidatorStub,lookupNormalizerStub, describerStub, serviceProviderStub, loggerStub);
    
        _controller = new CompaniesController(_bll, _userManager, mapperWeb, null);

        _userManager.GetUserId(_controller.User).Returns(Guid.NewGuid().ToString());

    }
    [Fact]
    public async Task GetTest()
    {
        var res = await _controller.GetCompanies();
        var okRes = res.Result as OkObjectResult;
        var val = okRes!.Value as IEnumerable<DTO.v1_0.Company>;
        Assert.Empty(val); 
    }
}