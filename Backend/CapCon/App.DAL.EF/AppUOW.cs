using App.Contracts.DAL;
using App.Contracts.DAL.Repositories;
using App.DAL.EF.Repositories;
using App.Domain.Identity;
using AutoMapper;
using Base.Contracts.DAL;
using Base.DAL.EF;

namespace App.DAL.EF;

public class AppUOW : BaseUnitOfWork<AppDbContext>, IAppUnitOfWork
{
    private readonly IMapper _mapper;

    public AppUOW(AppDbContext dbContext, IMapper mapper) : base(dbContext)
    {
        _mapper = mapper;
    }

    private ICompanyRepository? _companies;
    
    public ICompanyRepository Companies =>
        _companies ?? new CompanyRepository(UowDbContext, _mapper);


    private ILoanOfferRepository? _loanOffers;
    public ILoanOfferRepository LoanOffers =>
        _loanOffers ?? new LoanOfferRepository(UowDbContext, _mapper);


    private ILoanRepository? _loans;
    public ILoanRepository Loans =>
        _loans ?? new LoanRepository(UowDbContext, _mapper);


    private ILoanRequestRepository? _loanRequests;

    public ILoanRequestRepository LoanRequests =>
        _loanRequests ?? new LoanRequestRepository(UowDbContext, _mapper);


    private ITransactionRepository? _transactions;

    public ITransactionRepository Transactions =>
        _transactions ?? new TransactionRepository(UowDbContext, _mapper);
    


    private IUserCompanyRepository? _userCompanies;

    public IUserCompanyRepository UserCompanies =>
        _userCompanies ?? new UserCompanyRepository(UowDbContext, _mapper);

    private IEntityRepository<AppUser>? _users;

    public IEntityRepository<AppUser> Users => _users ??
                                      new BaseEntityRepository<AppUser, AppUser, AppDbContext>(UowDbContext,
                                          new DalDomainMapper<AppUser, AppUser>(_mapper));
}