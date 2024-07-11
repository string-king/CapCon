using App.Contracts.DAL.Repositories;
using AutoMapper;
using APPDomain = App.Domain;
using DALDTO = App.DAL.DTO;
using Base.Contracts.DAL;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories;

public class LoanRepository : 
    BaseEntityRepository<APPDomain.Loan, DALDTO.Loan, AppDbContext>, ILoanRepository
{
    private readonly IMapper _mapper;
    
    public LoanRepository(AppDbContext dbContext, IMapper mapper) : 
        base(dbContext, new DalDomainMapper<APPDomain.Loan, DALDTO.Loan>(mapper))
    {
        _mapper = mapper;
    }

    public async Task<IEnumerable<DALDTO.Loan>> GetAllLoansIncludingAsync(Guid userId = default, bool noTracking = true)
    {
        var query = CreateQuery(userId, noTracking);
        query = query.Include(l => l.AppUser)
            .Include(l => l.Transactions)
            .Include(l => l.Company);
        
        return (await query.ToListAsync()).Select(l => _mapper.Map<DALDTO.Loan>(l));
        
    }

    public async Task<DALDTO.Loan?> FirstOrDefaultLoanIncludingAsync(Guid id, Guid userId = default, bool noTracking = true)
    {
        var query = CreateQuery(userId, noTracking);
        
        query = query.Include(l => l.AppUser)
            .Include(l => l.Transactions)
            .Include(l => l.Company);
        
        var res = await query.FirstOrDefaultAsync(l => l.Id == id);

        return _mapper.Map<DALDTO.Loan>(res);
    }
}