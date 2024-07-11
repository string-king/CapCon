using App.BLL.DTO;
using App.BLL.DTO.Enums;
using App.BLL.Services;
using App.Contracts.BLL;
using App.Contracts.BLL.Services;
using App.Contracts.DAL;
using App.DAL.EF;
using AutoMapper;
using Base.BLL;

namespace App.BLL;

public class AppBLL : BaseBLL<AppDbContext>, IAppBLL
{
    private readonly IMapper _mapper;
    private readonly IAppUnitOfWork _uow;
    
    public AppBLL(IAppUnitOfWork uoW, IMapper mapper) : base(uoW)
    {
        _mapper = mapper;
        _uow = uoW;
    }

    private ICompanyService? _companies;
    public ICompanyService Companies => 
        _companies ?? new CompanyService(_uow, _uow.Companies, _mapper);
    
    
    private ILoanOfferService? _loanOffers;
    public ILoanOfferService LoanOffers => 
        _loanOffers ?? new LoanOfferService(_uow, _uow.LoanOffers, _mapper);
    
    private ILoanRequestService? _loanRequests;
    public ILoanRequestService LoanRequests => 
        _loanRequests ?? new LoanRequestService(_uow, _uow.LoanRequests, _mapper);
    
    private ILoanService? _loans;
    public ILoanService Loans => 
        _loans ?? new LoanService(_uow, _uow.Loans, _mapper);
    
    private ITransactionService? _transactions;

    public ITransactionService Transactions =>
        _transactions ?? new TransactionService(_uow, _uow.Transactions, _mapper);
    
    
    private IUserCompanyService? _userCompanies;
    public IUserCompanyService UserCompanies => 
        _userCompanies ?? new UserCompanyService(_uow, _uow.UserCompanies, _mapper);

    public async Task<Transaction?> AddTransactionToLoanAsync(Guid loanId, Transaction transaction, Guid userId)
    {
        var loan = await Loans.FirstOrDefaultLoanIncludingAsync(loanId, noTracking: true);
        var loanToUpdate = await Loans.FirstOrDefaultAsync(loanId, noTracking: true);
        if (loan == null)
        {
            throw new Exception("Loan not found!");
        }

        if (transaction.TransactionType == ETransactionType.LenderToBorrower && loan.AppUserId != userId)
        {
            throw new Exception("You can't add Lender to borrower transaction to loan you don't own!");
        }

        if (loan!.Transactions == null || loan!.Transactions!.Count == 0)
        {
            if (transaction.TransactionType == ETransactionType.BorrowerToLender)
            {
                throw new Exception("First transaction must be Lender to Borrower!");
            }

            if (transaction.Amount != loan!.Amount)
            {
                throw new Exception("First transaction amount must be equal to loan amount!");
            }

            loanToUpdate!.Active = true;
            loanToUpdate!.StartDate = DateTime.Now.ToUniversalTime();
        }

        var borrowerToLenderTransactions = loan.Transactions!.Where(t => t.TransactionType.Equals(ETransactionType.BorrowerToLender)).ToList();
        var loanBalance = loan.Amount * (1 + loan.Interest / 100) - borrowerToLenderTransactions.Sum(t => t.Amount);

        if (transaction.Amount > loanBalance)
        {
            throw new Exception("Transaction amount can't be greater than loan balance!");
        }

        double tolerance = 0.01;

        if (Math.Abs(transaction.Amount - loanBalance) < (decimal)tolerance && transaction.TransactionType == ETransactionType.BorrowerToLender)
        {
            loanToUpdate!.Active = false;
        }
        transaction.Date = DateTime.Now.ToUniversalTime();
        Transactions.Add(transaction);
        Loans.Update(loanToUpdate!);
        await SaveChangesAsync();

        return transaction;
    }
}