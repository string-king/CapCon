using DALDTO = App.DAL.DTO;
using Base.Contracts.DAL;

namespace App.Contracts.DAL.Repositories;

public interface ICompanyRepository : IEntityRepository<DALDTO.Company>, ICompanyRepositoryCustom<DALDTO.Company>
{
    // define custom DAL only methods here
    
}

// define shared (w bll) custom methods here
public interface ICompanyRepositoryCustom<TEntity>
{
    Task<IEnumerable<TEntity>> GetAllSortedAsync(Guid userId, bool noTracking = true);
    
    Task<TEntity> AddCompanyWithManagerAsync(TEntity entity, Guid userId);
    
    Task<IEnumerable<TEntity>> GetAllCompaniesIncludingAsync(Guid userId = default, bool noTracking = true);
    Task<TEntity?> FirstOrDefaultCompanyIncludingAsync(Guid id, Guid userId = default, bool noTracking = true);

}