using System.ComponentModel.DataAnnotations;
using Base.Contracts.Domain;
using Base.Domain;

namespace App.DAL.DTO;

public class Company : IDomainEntityId
{
    public Guid Id { get; set; }
    
    public string CompanyName { get; set; } = default!;
    
    public ICollection<UserCompany>? UserCompanies { get; set; }
    public ICollection<LoanRequest>? LoanRequests { get; set; }
    public ICollection<LoanOffer>? LoanOffers { get; set; }
    public ICollection<Loan>? Loans { get; set; }
    
}