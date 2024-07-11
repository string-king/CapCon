using App.Contracts.DAL.Repositories;
using AutoMapper;
using APPDomain = App.Domain;
using DALDTO = App.DAL.DTO;
using Base.Contracts.DAL;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories;

public class UserCompanyRepository : 
    BaseEntityRepository<APPDomain.UserCompany, DALDTO.UserCompany, AppDbContext>, IUserCompanyRepository
{
    private readonly IMapper _mapper;

    
    public UserCompanyRepository(AppDbContext dbContext, IMapper mapper) : 
        base(dbContext, new DalDomainMapper<APPDomain.UserCompany, DALDTO.UserCompany>(mapper))
    {
        _mapper = mapper;
    }

    public async Task<IEnumerable<DALDTO.UserCompany>> GetAllUserCompaniesIncludingAsync(Guid userId = default, bool noTracking = true)
    {
        var query = CreateQuery(userId, false);

        query = query.Include(uc => uc.AppUser)
            .Include(uc => uc.Company)
            .ThenInclude(c => c!.UserCompanies)
            .Include(uc => uc.Company)
            .ThenInclude(c => c!.Loans)
            .Include(uc => uc.Company)
            .ThenInclude(c => c!.LoanOffers)
            .Include(uc => uc.Company)
            .ThenInclude(c => c!.LoanRequests);

        return (await query.ToListAsync()).Select(e => Mapper.Map(e));
    }

    public async Task<DALDTO.UserCompany?> FirstOrDefaultUserCompanyIncludingAsync(Guid id, Guid userId = default,
        bool noTracking = true)
    {
        var query = CreateQuery(userId, false);

        query = query.Include(uc => uc.AppUser)
            .Include(uc => uc.Company)
            .ThenInclude(c => c!.UserCompanies)
            .Include(uc => uc.Company)
            .ThenInclude(c => c!.Loans)
            .Include(uc => uc.Company)
            .ThenInclude(c => c!.LoanOffers)
            .Include(uc => uc.Company)
            .ThenInclude(c => c!.LoanRequests);

        return Mapper.Map(await query.FirstOrDefaultAsync(c => c.Id.Equals(id)));
    }
}