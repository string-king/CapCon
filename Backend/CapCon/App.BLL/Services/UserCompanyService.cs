using App.Contracts.BLL.Services;
using App.Contracts.DAL;
using App.Contracts.DAL.Repositories;
using App.DAL.DTO;
using App.DAL.EF;
using App.DAL.EF.Repositories;
using AutoMapper;
using Base.BLL;
using Base.Contracts.BLL;
using Company = App.BLL.DTO.Company;
using UserCompany = App.BLL.DTO.UserCompany;

namespace App.BLL.Services;

public class UserCompanyService : 
    BaseEntityService<App.DAL.DTO.UserCompany, App.BLL.DTO.UserCompany, IUserCompanyRepository>,
    IUserCompanyService
{
    private readonly IAppUnitOfWork _uow;
    private readonly IMapper _mapper;
    public UserCompanyService(IAppUnitOfWork uoW, IUserCompanyRepository repository, IMapper mapper) : base(uoW, repository, new BllDalMapper<App.DAL.DTO.UserCompany, App.BLL.DTO.UserCompany>(mapper))
    {
        _mapper = mapper;
        _uow = uoW;
    }
    
    public async Task<IEnumerable<UserCompany>> GetAllUserCompaniesIncludingAsync(Guid userId = default, bool noTracking = true)
    {
        return (await _uow.UserCompanies.GetAllUserCompaniesIncludingAsync(userId)).Select(e => Mapper.Map(e));
    }

    public async Task<UserCompany?> FirstOrDefaultUserCompanyIncludingAsync(Guid id, Guid userId = default,
        bool noTracking = true)
    {
        return _mapper.Map<UserCompany>(await _uow.UserCompanies.FirstOrDefaultUserCompanyIncludingAsync(id, userId, noTracking));
    }

    public async Task<UserCompany> GetUserCompanyIncludingAsync(Guid id, Guid userId = default, bool noTracking = true)
    {
        return _mapper.Map<UserCompany>(await _uow.UserCompanies.FirstOrDefaultUserCompanyIncludingAsync(id, userId, noTracking));
    }
    
}