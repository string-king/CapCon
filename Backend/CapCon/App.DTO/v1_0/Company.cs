using System.ComponentModel.DataAnnotations;
using Base.Contracts.Domain;

namespace App.DTO.v1_0;

public class Company
{
    public Guid Id { get; set; }

    [MinLength(1, ErrorMessage = "Company name must be at least 1 character!"),
     MaxLength(128, ErrorMessage = "Company name must be less than 128 characters!")]
    public string CompanyName { get; set; } = default!;
    public ICollection<UserCompanyForCompany>? UserCompanies { get; set; }
    public ICollection<LoanRequest>? LoanRequests { get; set; }
    public ICollection<LoanOffer>? LoanOffers { get; set; }
    public ICollection<Loan>? Loans { get; set; }
}

public class CompanySimple
{
    public Guid Id { get; set; }
    public string CompanyName { get; set; } = default!;
}
