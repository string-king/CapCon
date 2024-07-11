using Base.Contracts.DAL;
using Base.DAL.EF;
using Base.Test.Domain;

namespace Base.Test.DAL;

public class TestEntityRepository : BaseEntityRepository<TestEntity, TestEntity, TestDbContext>
{
    public TestEntityRepository(TestDbContext dbContext, IDalMapper<TestEntity, TestEntity> mapper) : base(dbContext, mapper)
    {
    }
}