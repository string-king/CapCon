using App.Contracts.DAL.Repositories;
using Base.Contracts.DAL;

namespace App.Contracts.BLL.Services;

public interface ICompanyService : IEntityRepository<App.BLL.DTO.Company>, ICompanyRepositoryCustom<App.BLL.DTO.Company>
{
    
}