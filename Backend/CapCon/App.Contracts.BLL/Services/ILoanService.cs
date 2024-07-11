using App.BLL.DTO;
using App.Contracts.DAL.Repositories;
using Base.Contracts.DAL;

namespace App.Contracts.BLL.Services;

public interface ILoanService : IEntityRepository<App.BLL.DTO.Loan>, ILoanRepositoryCustom<App.BLL.DTO.Loan>
{
   
}