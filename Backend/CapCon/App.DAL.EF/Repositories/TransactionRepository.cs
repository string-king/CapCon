using App.Contracts.DAL.Repositories;
using AutoMapper;
using APPDomain = App.Domain;
using DALDTO = App.DAL.DTO;
using Base.Contracts.DAL;
using Base.DAL.EF;

namespace App.DAL.EF.Repositories;

public class TransactionRepository : 
    BaseEntityRepository<APPDomain.Transaction, DALDTO.Transaction, AppDbContext>, ITransactionRepository
{
    
    public TransactionRepository(AppDbContext dbContext, IMapper mapper) : 
        base(dbContext, new DalDomainMapper<APPDomain.Transaction, DALDTO.Transaction>(mapper))
    {
    }
}