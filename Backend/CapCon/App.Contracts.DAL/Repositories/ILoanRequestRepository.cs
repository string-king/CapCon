using DALDTO = App.DAL.DTO;
using Base.Contracts.DAL;

namespace App.Contracts.DAL.Repositories;

public interface ILoanRequestRepository : IEntityRepository<DALDTO.LoanRequest>, ILoanRequestRepositoryCustom<DALDTO.LoanRequest>
{
}

public interface ILoanRequestRepositoryCustom<TEntity>
{
    Task<IEnumerable<TEntity>> GetAllActiveLoanRequestsIncludingAsync(Guid userId = default, bool noTracking = true);
    Task<IEnumerable<TEntity>> GetAllLoanRequestsIncludingAsync(Guid userId = default, bool noTracking = true);

    Task<TEntity?> FirstOrDefaultLoanRequestIncludingAsync(Guid id, Guid userId = default, bool noTracking = true);
}