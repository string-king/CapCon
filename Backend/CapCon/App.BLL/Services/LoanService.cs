using App.BLL.DTO.Enums;
using App.Contracts.BLL.Services;
using App.Contracts.DAL;
using App.Contracts.DAL.Repositories;
using App.DAL.DTO;
using App.DAL.EF;
using App.DAL.EF.Repositories;
using AutoMapper;
using Base.BLL;
using Base.Contracts.BLL;
using Loan = App.BLL.DTO.Loan;
using Transaction = App.BLL.DTO.Transaction;

namespace App.BLL.Services;

public class LoanService : 
    BaseEntityService<App.DAL.DTO.Loan, App.BLL.DTO.Loan, ILoanRepository>,
    ILoanService
{
    private readonly IAppUnitOfWork _uow;
    private readonly IMapper _mapper;
    public LoanService(IAppUnitOfWork uoW, ILoanRepository repository, IMapper mapper) : base(uoW, repository, new BllDalMapper<App.DAL.DTO.Loan, App.BLL.DTO.Loan>(mapper))
    {
        _mapper = mapper;
        _uow = uoW;
    }

    public async Task<IEnumerable<Loan>> GetAllLoansIncludingAsync(Guid userId = default, bool noTracking = true)
    {
        return (await _uow.Loans.GetAllLoansIncludingAsync(userId, noTracking)).Select(l => _mapper.Map<Loan>(l));
    }

    public async Task<Loan?> FirstOrDefaultLoanIncludingAsync(Guid id, Guid userId = default, bool noTracking = true)
    {
        return _mapper.Map<Loan>(await _uow.Loans.FirstOrDefaultLoanIncludingAsync(id, userId, noTracking));

    }
    
}