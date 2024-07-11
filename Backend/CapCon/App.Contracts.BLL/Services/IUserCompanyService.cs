using App.Contracts.DAL.Repositories;
using Base.Contracts.DAL;

namespace App.Contracts.BLL.Services;

public interface IUserCompanyService : IEntityRepository<App.BLL.DTO.UserCompany>, IUserCompanyRepositoryCustom<App.BLL.DTO.UserCompany>
{
    
}