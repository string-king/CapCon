using App.BLL.DTO;
using App.Contracts.BLL.Services;
using Base.Contracts.BLL;

namespace App.Contracts.BLL;

public interface IAppBLL : IBLL
{
    ICompanyService Companies { get; }
    ILoanOfferService LoanOffers { get; }
    ILoanRequestService LoanRequests { get; }
    ILoanService Loans { get; }
    ITransactionService Transactions { get; }
    IUserCompanyService UserCompanies { get; }
    
    Task<Transaction?> AddTransactionToLoanAsync(Guid loanId, Transaction transaction, Guid userId);
}