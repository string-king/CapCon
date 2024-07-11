using AutoMapper;
using Base.DAL.EF;
using Base.Test.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Base.Test.DAL;

public class BaseRepositoryTest
{
    private readonly TestDbContext _ctx;
    private readonly TestEntityRepository _testEntityRepository;

    public BaseRepositoryTest()
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
    }
    
    [Fact]
    public async Task TestAdd()
    {
        // Arrange
        var entity = new TestEntity() { Value = "Foo" };

        // Act
        var addedEntity = _testEntityRepository.Add(entity);
        await _ctx.SaveChangesAsync();

        // Assert
        Assert.NotNull(addedEntity);
        Assert.Equal("Foo", addedEntity.Value);
    }

    [Fact]
    public async Task TestUpdate()
    {
        // Arrange
        var entity = new TestEntity() { Value = "Foo" };
        _testEntityRepository.Add(entity);
        await _ctx.SaveChangesAsync();

        // Act
        entity.Value = "Bar";
        var updatedEntity = _testEntityRepository.Update(entity);
        await _ctx.SaveChangesAsync();

        // Assert
        Assert.NotNull(updatedEntity);
        Assert.Equal("Bar", updatedEntity.Value);
    }

    /*[Fact]
    public async Task TestRemove()
    {
        // Arrange
        var entity = new TestEntity() { Value = "Foo"};
        _testEntityRepository.Add(entity);
        await _ctx.SaveChangesAsync();

        // Act
        await _testEntityRepository.RemoveAsync(entity);
        await _ctx.SaveChangesAsync();

        // Assert
        Assert.False(_testEntityRepository.Exists(entity.Id));
    }*/

    [Fact]
    public async Task TestGetAllAsync()
    {
        // Arrange
        var entity1 = new TestEntity() { Value = "Foo" };
        var entity2 = new TestEntity() { Value = "Bar" };
        _testEntityRepository.Add(entity1);
        _testEntityRepository.Add(entity2);
        await _ctx.SaveChangesAsync();

        // Act
        var entities = await _testEntityRepository.GetAllAsync();

        // Assert
        Assert.Equal(2, entities.Count());
    }
    
    [Fact]
    public async Task TestGetAll()
    {
        // Arrange
        var entity1 = new TestEntity() { Value = "Foo" };
        var entity2 = new TestEntity() { Value = "Bar" };
        _testEntityRepository.Add(entity1);
        _testEntityRepository.Add(entity2);
        await _ctx.SaveChangesAsync();

        // Act
        var entities = _testEntityRepository.GetAll();

        // Assert
        Assert.Equal(2, entities.Count());
    }

    [Fact]
    public async Task TestExists()
    {
        // Arrange
        var entity = new TestEntity() { Value = "Foo" };
        var e = _testEntityRepository.Add(entity);
        await _ctx.SaveChangesAsync();

        // Act
        var exists = _testEntityRepository.Exists(e.Id);

        // Assert
        Assert.True(exists);
    }

    [Fact]
    public async Task TestFirstOrDefault()
    {
        // Arrange
        var entity = new TestEntity() { Value = "Foo" };
        var e = _testEntityRepository.Add(entity);
        await _ctx.SaveChangesAsync();

        // Act
        var firstEntity = await _testEntityRepository.FirstOrDefaultAsync(e.Id);

        // Assert
        Assert.NotNull(firstEntity);
        Assert.Equal("Foo", firstEntity.Value);
    }

    [Fact]
    public async Task TestGetAllWithUser()
    {
        // Arrange
        var entity1 = new TestEntity() { Value = "Foo" };
        var entity2 = new TestEntity() { Value = "Bar" };
        _testEntityRepository.Add(entity1);
        _testEntityRepository.Add(entity2);

        var user = new IdentityUser<Guid>() { UserName = "test" };
        _ctx.Users.Add(user);
        var entity3 = new TestEntity() { Value = "Baz", AppUserId = user.Id };
        _testEntityRepository.Add(entity3);
        await _ctx.SaveChangesAsync();

        // Act
        var entities = _testEntityRepository.GetAll(user.Id);

        // Assert
        Assert.Equal(1, entities.Count());
    }
    
    [Fact]
    public async Task TestExistWithUser()
    {
        // Arrange
        var user = new IdentityUser<Guid>() { UserName = "test" };
        _ctx.Users.Add(user);
        var e1ID = Guid.NewGuid();
        var e2ID = Guid.NewGuid();
        var entity1 = new TestEntity() { Id = e1ID, Value = "Foo" };
        var entity2 = new TestEntity() { Id = e2ID, Value = "Baz", AppUserId = user.Id };
        _testEntityRepository.Add(entity1);
        _testEntityRepository.Add(entity2);
        
        await _ctx.SaveChangesAsync();

        // Act
        var exists1 = await _testEntityRepository.ExistsAsync(e1ID, user.Id);
        
        var exists2 = await _testEntityRepository.ExistsAsync(e2ID, user.Id);

        // Assert
        Assert.False(exists1);
        Assert.True(exists2);
    }
    
    [Fact]
    public async Task TestFirstOrDefaultWithUser()
    {
        // Arrange
        var user = new IdentityUser<Guid>() { UserName = "test" };
        _ctx.Users.Add(user);
        var e1ID = Guid.NewGuid();
        var e2ID = Guid.NewGuid();
        var entity1 = new TestEntity() { Id = e1ID, Value = "Foo" };
        var entity2 = new TestEntity() { Id = e2ID, Value = "Baz", AppUserId = user.Id };
        _testEntityRepository.Add(entity1);
        _testEntityRepository.Add(entity2);
        
        await _ctx.SaveChangesAsync();

        // Act
        var e1 = await _testEntityRepository.FirstOrDefaultAsync(e1ID, user.Id);
        
        var e2 = _testEntityRepository.FirstOrDefault(e2ID, user.Id);

        // Assert
        Assert.Null(e1);
        Assert.NotNull(e2);
    }
    
    [Fact]
    public async Task TestRemoveAsyncWithUser()
    {
        var id = Guid.NewGuid();
        // Arrange
        var user = new IdentityUser<Guid>() { UserName = "test" };
        _ctx.Users.Add(user);

        var entity1 = new TestEntity() { Id = Guid.NewGuid(), Value = "Foo" };
        var entity2 = new TestEntity() { Id = Guid.NewGuid(), Value = "Bar", AppUserId = user.Id };
        _testEntityRepository.Add(entity1);
        _testEntityRepository.Add(entity2);

        await _ctx.SaveChangesAsync();

        // Detach entities
        _ctx.Entry(entity1).State = EntityState.Detached;
        _ctx.Entry(entity2).State = EntityState.Detached;

        // Act
        await _testEntityRepository.RemoveAsync(entity2.Id, user.Id);
        await _testEntityRepository.RemoveAsync(entity1.Id);
        await _testEntityRepository.RemoveAsync(entity1.Id);
        await _ctx.SaveChangesAsync();

        // Assert
        Assert.False(_testEntityRepository.Exists(entity2.Id));
        Assert.False(_testEntityRepository.Exists(entity1.Id));
    }
    
    [Fact]
    async Task TestRemoveAsync()
    {
        // Arrange
        var user = new IdentityUser<Guid>() { UserName = "test" };
        _ctx.Users.Add(user);
        var entity = new TestEntity() { Id = Guid.NewGuid(), Value = "Foo"};
        var enity2 = new TestEntity() { Id = Guid.NewGuid(), Value = "Bar", AppUserId = user.Id};
        _testEntityRepository.Add(entity);
        _testEntityRepository.Add(enity2);
        await _ctx.SaveChangesAsync();

        // Act
        await _testEntityRepository.RemoveAsync(entity);
        await _testEntityRepository.RemoveAsync(entity);
        await _testEntityRepository.RemoveAsync(enity2, user.Id);
        await _ctx.SaveChangesAsync();

        // Assert
        Assert.False(_testEntityRepository.Exists(entity.Id));
    }
    
    [Fact]
    public async Task TestRemoveWithUser()
    {
        // Arrange
        var user = new IdentityUser<Guid>() { UserName = "test" };
        _ctx.Users.Add(user);
        
        var entity1 = new TestEntity() { Id = Guid.NewGuid(), Value = "Foo" };
        var entity2 = new TestEntity() { Id = Guid.NewGuid(), Value = "Bar", AppUserId = user.Id };
        _testEntityRepository.Add(entity1);
        _testEntityRepository.Add(entity2);
        
        await _ctx.SaveChangesAsync();
        
        // Act
        _testEntityRepository.Remove(entity2.Id, user.Id);
        _testEntityRepository.Remove(entity1.Id);
        _testEntityRepository.Remove(entity1.Id);

        await _ctx.SaveChangesAsync();
        
        // Assert
        Assert.False(_testEntityRepository.Exists(entity2.Id));
        Assert.False(_testEntityRepository.Exists(entity1.Id));
    }
    
    [Fact]
    async Task TestRemove()
    {
        // Arrange
        var user = new IdentityUser<Guid>() { UserName = "test" };
        _ctx.Users.Add(user);
        var entity = new TestEntity() { Id = Guid.NewGuid(), Value = "Foo"};
        var enity2 = new TestEntity() { Id = Guid.NewGuid(), Value = "Bar", AppUserId = user.Id};
        _testEntityRepository.Add(entity);
        _testEntityRepository.Add(enity2);
        await _ctx.SaveChangesAsync();

        // Act
         _testEntityRepository.Remove(entity);
         _testEntityRepository.Remove(entity);
         _testEntityRepository.Remove(enity2, user.Id);
        await _ctx.SaveChangesAsync();

        // Assert
        Assert.False(_testEntityRepository.Exists(entity.Id));
    }


}