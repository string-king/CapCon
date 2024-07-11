using Base.Contracts.DAL;

namespace App.Contracts.BLL.Services;

public interface ITransactionService : IEntityRepository<App.BLL.DTO.Transaction>
{
    
}