using App.Contracts.DAL.Repositories;
using App.Domain.Identity;
using Base.Contracts.DAL;

namespace App.Contracts.DAL;

public interface IAppUnitOfWork : IUnitOfWork
{
    
    ICompanyRepository Companies { get; }
    ILoanOfferRepository LoanOffers { get; }
    ILoanRepository Loans { get; }
    ILoanRequestRepository LoanRequests { get; }
    ITransactionRepository Transactions { get; }
    IUserCompanyRepository UserCompanies { get; }
    IEntityRepository<AppUser> Users { get; }
}