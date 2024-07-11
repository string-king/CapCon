using App.Contracts.DAL.Repositories;
using Base.Contracts.DAL;

namespace App.Contracts.BLL.Services;

public interface ILoanRequestService : IEntityRepository<App.BLL.DTO.LoanRequest>, ILoanRequestRepositoryCustom<App.BLL.DTO.LoanRequest>
{
    
}