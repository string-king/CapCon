using Base.Contracts.BLL;
using Base.Contracts.DAL;
using Base.Contracts.Domain;

namespace Base.BLL;

public class BaseEntityService<TDalEntity, TBllEntity, TRepository> : 
    BaseEntityService<TDalEntity, TBllEntity, TRepository, Guid>,
    IEntityService<TBllEntity> 
    where TBllEntity : class, IDomainEntityId 
    where TDalEntity : class, IDomainEntityId<Guid> 
    where TRepository : IEntityRepository<TDalEntity, Guid>
{
    public BaseEntityService(IUnitOfWork uoW, TRepository repository, IBLLMapper<TDalEntity, TBllEntity> mapper) : 
        base(uoW, repository, mapper)
    {
    }
}

public class BaseEntityService<TDalEntity, TBllEntity, TRepository, TKey> : IEntityService<TBllEntity, TKey> 
    where TRepository : IEntityRepository<TDalEntity, TKey>
    where TBllEntity : class, IDomainEntityId<TKey> 
    where TKey : IEquatable<TKey>
    where TDalEntity : class, IDomainEntityId<TKey>
{
    private readonly IUnitOfWork UoW;
    protected readonly TRepository Repository;
    protected readonly IBLLMapper<TDalEntity, TBllEntity> Mapper;

    public BaseEntityService(IUnitOfWork uoW, TRepository repository, IBLLMapper<TDalEntity, TBllEntity> mapper)
    {
        UoW = uoW;
        Repository = repository;
        Mapper = mapper;
    }
    
    
    public TBllEntity Add(TBllEntity entity)
    {
        return Mapper.Map(Repository.Add(Mapper.Map(entity)))!;
    }

    public TBllEntity Update(TBllEntity entity)
    {
        return Mapper.Map(Repository.Update(Mapper.Map(entity)))!;
    }

    public int Remove(TBllEntity entity, TKey? userId = default)
    {
        return Repository.Remove(Mapper.Map(entity), userId);
    }

    public int Remove(TKey id, TKey? userId = default)
    {
        return Repository.Remove(id, userId);
    }

    public TBllEntity? FirstOrDefault(TKey id, TKey? userId = default, bool noTracking = true)
    {
        return Mapper.Map(Repository.FirstOrDefault(id, userId, noTracking));
    }

    public IEnumerable<TBllEntity> GetAll(TKey? userId = default, bool noTracking = true)
    {
        return Repository.GetAll(userId, noTracking).Select(e => Mapper.Map(e));
    }

    public bool Exists(TKey id, TKey? userId = default)
    {
        return Repository.Exists(id, userId);
    }

    public async Task<TBllEntity?> FirstOrDefaultAsync(TKey id, TKey? userId = default, bool noTracking = true)
    {
        return Mapper.Map(await Repository.FirstOrDefaultAsync(id, userId, noTracking));
    }

    public async Task<IEnumerable<TBllEntity>> GetAllAsync(TKey? userId = default, bool noTracking = true)
    {
        return (await Repository.GetAllAsync(userId, noTracking)).Select(e => Mapper.Map(e));
    }

    public async Task<bool> ExistsAsync(TKey id, TKey? userId = default)
    {
        return await Repository.ExistsAsync(id, userId);
    }

    public async Task<int> RemoveAsync(TBllEntity entity, TKey? userId = default)
    {
        return await Repository.RemoveAsync(Mapper.Map(entity), userId);
    }

    public async Task<int> RemoveAsync(TKey id, TKey? userId = default)
    {
        return await Repository.RemoveAsync(id, userId);

    }
}