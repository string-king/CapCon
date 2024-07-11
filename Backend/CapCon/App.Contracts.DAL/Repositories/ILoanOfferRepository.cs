using DALDTO = App.DAL.DTO;
using Base.Contracts.DAL;

namespace App.Contracts.DAL.Repositories;

public interface ILoanOfferRepository : IEntityRepository<DALDTO.LoanOffer>, ILoanOfferRepositoryCustom<DALDTO.LoanOffer>
{
    
}

public interface ILoanOfferRepositoryCustom<TEntity>
{
    Task<IEnumerable<TEntity>> GetAllLoanOffersIncludingAsync(Guid userId = default, bool noTracking = true);
    Task<TEntity?> FirstOrDefaultLoanOfferIncludingAsync(Guid id, Guid userId = default, bool noTracking = true);
}