using App.Contracts.BLL.Services;
using App.Contracts.DAL;
using App.Contracts.DAL.Repositories;
using App.DAL.DTO;
using App.DAL.EF;
using App.DAL.EF.Repositories;
using AutoMapper;
using Base.BLL;
using Base.Contracts.BLL;
using LoanRequest = App.BLL.DTO.LoanRequest;

namespace App.BLL.Services;

public class LoanRequestService : 
    BaseEntityService<App.DAL.DTO.LoanRequest, App.BLL.DTO.LoanRequest, ILoanRequestRepository>,
    ILoanRequestService
{
    private readonly IAppUnitOfWork _uow;
    private readonly IMapper _mapper;
    public LoanRequestService(IAppUnitOfWork uoW, ILoanRequestRepository repository, IMapper mapper) : base(uoW, repository, new BllDalMapper<App.DAL.DTO.LoanRequest, App.BLL.DTO.LoanRequest>(mapper))
    {
        _uow = uoW;
        _mapper = mapper;
    }

    public async Task<IEnumerable<LoanRequest>> GetAllActiveLoanRequestsIncludingAsync(Guid userId = default, bool noTracking = true)
    {
        return (await _uow.LoanRequests.GetAllActiveLoanRequestsIncludingAsync()).Select(e => Mapper.Map(e));
    }
    
    public async Task<IEnumerable<LoanRequest>> GetAllLoanRequestsIncludingAsync(Guid userId = default, bool noTracking = true)
    {
        return (await _uow.LoanRequests.GetAllLoanRequestsIncludingAsync()).Select(e => Mapper.Map(e));
    }

    public async Task<LoanRequest?> FirstOrDefaultLoanRequestIncludingAsync(Guid id, Guid userId = default, bool noTracking = true)
    {
        return _mapper.Map<LoanRequest>(await _uow.LoanRequests.FirstOrDefaultLoanRequestIncludingAsync(id: id, noTracking: noTracking));
    }
}