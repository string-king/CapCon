using App.Contracts.DAL.Repositories;
using Base.Contracts.DAL;

namespace App.Contracts.BLL.Services;

public interface ILoanOfferService : IEntityRepository<App.BLL.DTO.LoanOffer>, ILoanOfferRepositoryCustom<App.BLL.DTO.LoanOffer>
{
    
}