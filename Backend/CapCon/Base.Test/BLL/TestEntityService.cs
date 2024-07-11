using App.Contracts.DAL;
using AutoMapper;
using Base.BLL;
using Base.Contracts.BLL;
using Base.Contracts.DAL;
using Base.Test.DAL;
using Base.Test.Domain;

namespace Base.Test.BLL;

public class TestEntityService: BaseEntityService<TestEntity,TestEntity,TestEntityRepository>
{
    public TestEntityService(IUnitOfWork uoW, TestEntityRepository repository, IBLLMapper<TestEntity, TestEntity> mapper) : base(uoW, repository, mapper)
    {
        
    }
}