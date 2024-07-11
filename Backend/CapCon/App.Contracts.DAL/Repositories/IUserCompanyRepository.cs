using DALDTO = App.DAL.DTO;
using Base.Contracts.DAL;

namespace App.Contracts.DAL.Repositories;

public interface IUserCompanyRepository : IEntityRepository<DALDTO.UserCompany>, IUserCompanyRepositoryCustom<DALDTO.UserCompany>
{
    // define custom DAL only methods here
}

public interface IUserCompanyRepositoryCustom<TEntity>
{
    Task<IEnumerable<TEntity>> GetAllUserCompaniesIncludingAsync(Guid userId = default, bool noTracking = true);
    Task<TEntity?> FirstOrDefaultUserCompanyIncludingAsync(Guid id, Guid userId = default, bool noTracking = true);
}
    