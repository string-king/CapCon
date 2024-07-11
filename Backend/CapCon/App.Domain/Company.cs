using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace App.Domain;

public class Company : BaseEntityId
{
    
    [MinLength(1, ErrorMessage = "Company name must be at least 1 character!"), MaxLength(128, ErrorMessage = "Company name must be less than 128 characters!")]
    public string CompanyName { get; set; } = default!;
    
    public ICollection<UserCompany>? UserCompanies { get; set; }
    public ICollection<LoanRequest>? LoanRequests { get; set; }
    public ICollection<LoanOffer>? LoanOffers { get; set; }
    public ICollection<Loan>? Loans { get; set; }
}