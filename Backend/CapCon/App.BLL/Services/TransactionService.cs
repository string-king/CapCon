using App.Contracts.BLL.Services;
using App.Contracts.DAL;
using App.Contracts.DAL.Repositories;
using App.DAL.DTO;
using App.DAL.EF;
using App.DAL.EF.Repositories;
using AutoMapper;
using Base.BLL;
using Base.Contracts.BLL;

namespace App.BLL.Services;

public class TransactionService : 
    BaseEntityService<App.DAL.DTO.Transaction, App.BLL.DTO.Transaction, ITransactionRepository>,
    ITransactionService
{
    public TransactionService(IAppUnitOfWork uoW, ITransactionRepository repository, IMapper mapper) : base(uoW, repository, new BllDalMapper<App.DAL.DTO.Transaction, App.BLL.DTO.Transaction>(mapper))
    {
    }
}