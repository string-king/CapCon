using App.Contracts.DAL.Repositories;
using AutoMapper;
using APPDomain = App.Domain;
using DALDTO = App.DAL.DTO;
using Base.Contracts.DAL;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories;

public class LoanRequestRepository : 
    BaseEntityRepository<APPDomain.LoanRequest, DALDTO.LoanRequest, AppDbContext>, ILoanRequestRepository
{
    private readonly IMapper _mapper;
    
    public LoanRequestRepository(AppDbContext dbContext, IMapper mapper) : 
        base(dbContext, new DalDomainMapper<APPDomain.LoanRequest, DALDTO.LoanRequest>(mapper))
    {
        _mapper = mapper;
    }


    public async Task<IEnumerable<DALDTO.LoanRequest>> GetAllActiveLoanRequestsIncludingAsync(Guid userId = default, bool noTracking = true)
    {
        var query = CreateQuery();
        query = query
            .Where(lr => lr.Active)
            .OrderBy(lr => lr.CreatedAt)
            .Reverse()
            .Include(lr => lr.Company)
            .Include(lr => lr.LoanOffers)
            .ThenInclude(lo => lo.AppUser);
        
        return (await query.ToListAsync()).Select(lr => _mapper.Map<DALDTO.LoanRequest>(lr));
    }
    
    public async Task<IEnumerable<DALDTO.LoanRequest>> GetAllLoanRequestsIncludingAsync(Guid userId = default, bool noTracking = true)
    {
        var query = CreateQuery();
        query = query
            .OrderBy(lr => lr.CreatedAt)
            .Reverse()
            .Include(lr => lr.Company)
            .Include(lr => lr.LoanOffers)
            .ThenInclude(lo => lo.AppUser);
        
        return (await query.ToListAsync()).Select(lr => _mapper.Map<DALDTO.LoanRequest>(lr));
    }

    public async Task<DALDTO.LoanRequest?> FirstOrDefaultLoanRequestIncludingAsync(Guid id, Guid userId = default, bool noTracking = true)
    {
        var query = CreateQuery();
        query = query
            .Include(lr => lr.Company)
            .Include(lr => lr.LoanOffers)
            .ThenInclude(lo => lo.AppUser);
            

        return Mapper.Map(await query.FirstOrDefaultAsync(lr => lr.Id == id));
    }
}