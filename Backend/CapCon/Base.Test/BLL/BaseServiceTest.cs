using App.BLL;
using App.BLL.DTO.Identity;
using App.Contracts.DAL;
using AutoMapper;
using Base.DAL.EF;
using Base.Test.DAL;
using Base.Test.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Base.Test.BLL;

public class BaseServiceTest
{
    private readonly TestDbContext _ctx;
    private readonly TestEntityRepository _testEntityRepository;
    private readonly TestEntityService _testEntityService;
    private readonly TestUow _uow;
    

    public BaseServiceTest()
    {
        // set up mock database - inmemory
        var optionsBuilder = new DbContextOptionsBuilder<TestDbContext>();

        // use random guid as db instance id
        optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
        _ctx = new TestDbContext(optionsBuilder.Options);

        // reset db
        _ctx.Database.EnsureDeleted();
        _ctx.Database.EnsureCreated();

        var config = new MapperConfiguration(cfg => cfg.CreateMap<TestEntity, TestEntity>());
        var mapper = config.CreateMapper();

        _testEntityRepository =
            new TestEntityRepository(
                _ctx,
                new BaseDalDomainMapper<TestEntity, TestEntity>(mapper)
            );
        
        _uow = new TestUow(_ctx, mapper);
        
        _testEntityService = new TestEntityService(_uow, _testEntityRepository, new BllDalMapper<TestEntity, TestEntity>(mapper));
        
    }
    
    [Fact]
    public async Task TestAdd()
    {
        // Arrange
        var entity = new TestEntity() { Value = "Foo" };

        // Act
        entity = _testEntityService.Add(entity);
        await _uow.SaveChangesAsync();

        // Assert
        Assert.NotEmpty(_uow.TestEntities.GetAll());
        Assert.Equal("Foo", _uow.TestEntities.GetAll().First().Value);
    }

    [Fact]
    public async Task TestUpdate()
    {
        // Arrange
        var entity = new TestEntity() { Value = "Foo" };

        // Act
        entity = _testEntityService.Add(entity);
        await _uow.SaveChangesAsync();

        _ctx.ChangeTracker.Clear();

        var updatedEntity = _testEntityRepository.FirstOrDefault(entity.Id);

        updatedEntity!.Value = "Bar";
        _testEntityService.Update(updatedEntity);
        await _uow.SaveChangesAsync();

        // Assert
        Assert.NotEmpty(_testEntityService.GetAll());
        Assert.Equal("Bar", _uow.TestEntities.GetAll().First().Value);
    }
    
    [Fact]
    public async Task TestRemove()
    {
        var user = new IdentityUser<Guid>() { Email = "test" };
        _ctx.Users.Add(user);
        await _ctx.SaveChangesAsync();
        
        // Arrange
        var entity1 = new TestEntity() { AppUserId = user.Id, Value = "Foo" };
        var entity2 = new TestEntity() { AppUserId = user.Id, Value = "Bar" };

        // Act
        entity1 = _testEntityService.Add(entity1);
        entity2 = _testEntityService.Add(entity2);
        await _uow.SaveChangesAsync();

        _ctx.ChangeTracker.Clear();

        _testEntityService.Remove(entity1.Id);
        _testEntityService.Remove(entity2);
        await _uow.SaveChangesAsync();

        // Assert
        Assert.Empty(_testEntityService.GetAll(user.Id));
    }
    
    [Fact]
    public async Task TestRemoveAsync()
    {
        var user = new IdentityUser<Guid>() { Email = "test" };
        _ctx.Users.Add(user);
        await _ctx.SaveChangesAsync();
        
        // Arrange
        var entity1 = new TestEntity() { AppUserId = user.Id, Value = "Foo" };
        var entity2 = new TestEntity() { AppUserId = user.Id, Value = "Bar" };

        // Act
        entity1 = _testEntityService.Add(entity1);
        entity2 = _testEntityService.Add(entity2);
        await _uow.SaveChangesAsync();

        _ctx.ChangeTracker.Clear();

        await _testEntityService.RemoveAsync(entity1.Id);
        await _testEntityService.RemoveAsync(entity2);
        await _uow.SaveChangesAsync();

        // Assert
        Assert.Empty(_testEntityService.GetAll(user.Id));
    }
    
    [Fact]
    public async Task TestGetAllAsync()
    {
        // Arrange
        var user = new IdentityUser<Guid>() { Email = "test" };
        _ctx.Users.Add(user);
        await _ctx.SaveChangesAsync();
        
        var entity1 = new TestEntity() { AppUserId = user.Id, Value = "Foo" };
        var entity2 = new TestEntity() { AppUserId = user.Id, Value = "Bar" };

        // Act
        _testEntityService.Add(entity1);
        _testEntityService.Add(entity2);
        await _uow.SaveChangesAsync();

        // Assert
        Assert.NotEmpty(await _testEntityService.GetAllAsync());
        Assert.Equal("Foo", (await _testEntityService.GetAllAsync()).First().Value);
        Assert.Equal("Bar", (await _testEntityService.GetAllAsync(user.Id)).Last().Value);
    }
    
    [Fact]
    public async Task TestFirstOrDefault()
    {
        // Arrange
        var user = new IdentityUser<Guid>() { Email = "test" };
        _ctx.Users.Add(user);
        await _ctx.SaveChangesAsync();
        
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var entity1 = new TestEntity() { Id = id1, AppUserId = user.Id, Value = "Foo" };
        var entity2 = new TestEntity() { Id = id2, Value = "Bar" };

        // Act
        _testEntityService.Add(entity1);
        _testEntityService.Add(entity2);
        await _uow.SaveChangesAsync();

        // Assert
        Assert.False(_testEntityService.Exists(id2, user.Id));
        Assert.True(_testEntityService.Exists(id2));
        Assert.NotNull(_testEntityService.FirstOrDefault(id1));
        Assert.Null(_testEntityService.FirstOrDefault(id2, user.Id));
    }
    
    [Fact]
    public async Task TestFirstOrDefaultAsync()
    {
        // Arrange
        var user = new IdentityUser<Guid>() { Email = "test" };
        _ctx.Users.Add(user);
        await _ctx.SaveChangesAsync();
        
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var entity1 = new TestEntity() { Id = id1, AppUserId = user.Id, Value = "Foo" };
        var entity2 = new TestEntity() { Id = id2, Value = "Bar" };

        // Act
        _testEntityService.Add(entity1);
        _testEntityService.Add(entity2);
        await _uow.SaveChangesAsync();

        // Assert
        Assert.False(await _testEntityService.ExistsAsync(id2, user.Id));
        Assert.True(await _testEntityService.ExistsAsync(id2));
        Assert.NotNull( await _testEntityService.FirstOrDefaultAsync(id1));
        Assert.Null(await _testEntityService.FirstOrDefaultAsync(id2, user.Id));
    }
    
    
    
    
    
    




}