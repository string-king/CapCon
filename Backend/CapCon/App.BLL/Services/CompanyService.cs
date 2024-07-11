using App.Contracts.BLL.Services;
using App.Contracts.DAL;
using App.Contracts.DAL.Repositories;
using App.DAL.DTO.Enums;
using App.DAL.EF;
using App.DAL.EF.Repositories;
using AutoMapper;
using Base.BLL;
using Base.Contracts.BLL;
using Company = App.BLL.DTO.Company;
using UserCompany = App.DAL.DTO.UserCompany;

namespace App.BLL.Services;

public class CompanyService : 
    BaseEntityService<App.DAL.DTO.Company, App.BLL.DTO.Company, ICompanyRepository>,
    ICompanyService
{
    private readonly IAppUnitOfWork _uow;
    private readonly IMapper _mapper;
    public CompanyService(IAppUnitOfWork uoW, ICompanyRepository repository, IMapper mapper) : 
        base(uoW, repository, new BllDalMapper<App.DAL.DTO.Company, DTO.Company>(mapper))
    {
        _uow = uoW;
        _mapper = mapper;
    }

    public async Task<IEnumerable<Company>> GetAllSortedAsync(Guid userId, bool noTracking = true)
    {
        return (await Repository.GetAllSortedAsync(userId)).Select(e => Mapper.Map(e));
    }
    
    public async Task<Company> AddCompanyWithManagerAsync(Company companyDto, Guid userId)
    {
        var company = _mapper.Map<App.DAL.DTO.Company>(companyDto);

        company = _uow.Companies.Add(company);

        _uow.UserCompanies.Add(new UserCompany
        {
            AppUserId = userId,
            CompanyId = company.Id,
            Role = ECompanyRole.Manager
        });
        
        
        return _mapper.Map<Company>(company);
    }
    
    public async Task<IEnumerable<Company>> GetAllCompaniesIncludingAsync(Guid userId = default, bool noTracking = true)
    {
        return (await _uow.Companies.GetAllCompaniesIncludingAsync(userId)).Select(e => Mapper.Map(e));
    }

    public async Task<Company?> FirstOrDefaultCompanyIncludingAsync(Guid id, Guid userId = default, bool noTracking = true)
    {
        return _mapper.Map<Company>(await _uow.Companies.FirstOrDefaultCompanyIncludingAsync(id, userId, noTracking));
    }
}