using DALDTO = App.DAL.DTO;
using Base.Contracts.DAL;

namespace App.Contracts.DAL.Repositories;

public interface ILoanRepository : IEntityRepository<DALDTO.Loan>, ILoanRepositoryCustom<DALDTO.Loan>
{
}

public interface ILoanRepositoryCustom<TEntity>
{
    Task<IEnumerable<TEntity>> GetAllLoansIncludingAsync(Guid userId = default, bool noTracking = true);
    Task<TEntity?> FirstOrDefaultLoanIncludingAsync(Guid id, Guid userId = default, bool noTracking = true);
}