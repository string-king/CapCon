using System.ComponentModel.DataAnnotations;
using Base.Contracts.Domain;
using Base.Domain;
using Microsoft.AspNetCore.Identity;

namespace App.Domain.Identity;

public class AppUser : IdentityUser<Guid>, IDomainEntityId
{
    [MinLength(1), MaxLength(64)]
    public string FirstName { get; set; } = default!;

    [MinLength(1), MaxLength(64)]
    public string LastName { get; set; } = default!;
    
    
    public ICollection<Loan>? Loans { get; set; }
    
    public ICollection<UserCompany>? UserCompanies { get; set; }
    
    public ICollection<LoanOffer>? LoanOffers { get; set; }

    public ICollection<AppRefreshToken>? RefreshTokens { get; set; }
    
}