using System.ComponentModel.DataAnnotations;

namespace App.BLL.DTO.Identity;

public class AppUser
{
    public Guid Id { get; set; }
    [MinLength(1)]
    [MaxLength(64)]
    public string FirstName { get; set; } = default!;

    [MinLength(1)]
    [MaxLength(64)]
    public string LastName { get; set; } = default!;
    
    public string Email { get; set; } = default!;
    
    
    public ICollection<Loan>? Loans { get; set; }
    
    public ICollection<UserCompany>? UserCompanies { get; set; }
    
    public ICollection<LoanOffer>? LoanOffers { get; set; }

    public ICollection<AppRefreshToken>? RefreshTokens { get; set; }
}