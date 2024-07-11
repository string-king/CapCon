using App.Contracts.DAL.Repositories;
using AutoMapper;
using APPDomain = App.Domain;
using DALDTO = App.DAL.DTO;
using Base.Contracts.DAL;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories;

public class LoanOfferRepository : 
    BaseEntityRepository<APPDomain.LoanOffer, DALDTO.LoanOffer, AppDbContext>, ILoanOfferRepository
{
    
    public LoanOfferRepository(AppDbContext dbContext, IMapper mapper) : 
        base(dbContext, new DalDomainMapper<APPDomain.LoanOffer, DALDTO.LoanOffer>(mapper))
    {
    }

    public async Task<IEnumerable<DALDTO.LoanOffer>> GetAllLoanOffersIncludingAsync(Guid userId = default, bool noTracking = true)
    {
        var query = CreateQuery(userId, noTracking);

        query = query.Include(lo => lo.AppUser)
            .Include(lo => lo.LoanRequest)
            .ThenInclude(lr => lr!.Company);
        
        return (await query.ToListAsync()).Select(lo => Mapper.Map(lo));

    }

    public async Task<DALDTO.LoanOffer?> FirstOrDefaultLoanOfferIncludingAsync(Guid id, Guid userId = default, bool noTracking = true)
    {
        var query = CreateQuery(userId, noTracking);

        query = query.Include(lo => lo.AppUser)
            .Include(lo => lo.LoanRequest)
            .ThenInclude(lr => lr!.Company);
        
        return Mapper.Map(await query.FirstOrDefaultAsync(lo => lo.Id.Equals(id)))!;

    }
}