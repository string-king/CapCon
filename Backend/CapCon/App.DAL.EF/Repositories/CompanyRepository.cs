using App.Contracts.DAL.Repositories;
using App.Domain.Enums;
using AutoMapper;
using APPDomain = App.Domain;
using DALDTO = App.DAL.DTO;
using Base.Contracts.DAL;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories;

public class CompanyRepository : 
    BaseEntityRepository<APPDomain.Company, DALDTO.Company, AppDbContext>, ICompanyRepository
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    
    public CompanyRepository(AppDbContext dbContext, IMapper mapper) : 
        base(dbContext, new DalDomainMapper<APPDomain.Company, DALDTO.Company>(mapper))
    {
        _context = dbContext;
        _mapper = mapper;
    }

    public async Task<IEnumerable<DALDTO.Company>> GetAllSortedAsync(Guid userId, bool noTracking = true)
    {
        var query =
            CreateQuery(userId);
        query = query.OrderBy(c => c.CompanyName).Include(c => c.UserCompanies);
        return (await query.ToListAsync()).Select(e => Mapper.Map(e));
    }

    public async Task<DALDTO.Company> AddCompanyWithManagerAsync(DALDTO.Company companyDto, Guid userId)
    {
        var company = _mapper.Map<APPDomain.Company>(companyDto);

        _context.Companies.Add(company);

        _context.UserCompanies.Add(new APPDomain.UserCompany
        {
            AppUserId = userId,
            CompanyId = company.Id,
            Role = ECompanyRole.Manager
        });

        await _context.SaveChangesAsync();

        return _mapper.Map<DALDTO.Company>(company);
    }
    
    public virtual async Task<IEnumerable<DALDTO.Company>> GetAllCompaniesIncludingAsync(Guid userId = default, bool noTracking = true)
    {
        var query = CreateQuery(userId, noTracking);
        query = query
            .Include(c => c.UserCompanies)
            .ThenInclude(uc => uc.AppUser)
            .Include(c => c.Loans)
            .ThenInclude(l => l.AppUser)
            .Include(c => c.Loans)
            .ThenInclude(l => l.Transactions)
            .Include(c => c.LoanRequests)
            .ThenInclude(lr => lr.LoanOffers)
            .ThenInclude(lo => lo.AppUser);
        
        return (await query.ToListAsync()).Select(e => Mapper.Map(e));
    }

    public async Task<DALDTO.Company?> FirstOrDefaultCompanyIncludingAsync(Guid id, Guid userId = default, bool noTracking = true)
    {
        var query = CreateQuery(userId, noTracking);
        query = query
            .Include(c => c.UserCompanies)
            .ThenInclude(uc => uc.AppUser)
            .Include(c => c.Loans)
            .ThenInclude(l => l.AppUser)
            .Include(c => c.Loans)
            .ThenInclude(l => l.Transactions)
            .Include(c => c.LoanRequests)
            .ThenInclude(lr => lr.LoanOffers)
            .ThenInclude(lo => lo.AppUser);
            ;

        return Mapper.Map(await query.FirstOrDefaultAsync(c => c.Id.Equals(id)))!;
    }
}