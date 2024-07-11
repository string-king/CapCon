using App.BLL;
using AutoMapper;
using Base.Contracts.DAL;
using Base.DAL.EF;
using Base.Test.DAL;
using Base.Test.Domain;

namespace Base.Test.BLL;

public class TestUow : BaseUnitOfWork<TestDbContext>
{
    private readonly IMapper _mapper;
    public TestUow(TestDbContext dbContext, IMapper mapper) : base(dbContext)
    {
        _mapper = mapper;
    }
    
    private TestEntityRepository? _testEntities;
    
    public TestEntityRepository TestEntities =>
        _testEntities ?? new TestEntityRepository(UowDbContext, new BllDalMapper<TestEntity, TestEntity>(_mapper));
}