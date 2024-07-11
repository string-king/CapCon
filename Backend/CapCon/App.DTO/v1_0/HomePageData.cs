namespace App.DTO.v1_0;

public class HomePageData
{
    public ICollection<UserCompany> UserCompanies { get; set; } = default!;
    public ICollection<Loan> UserLoans { get; set; } = default!;
    public ICollection<LoanOffer> UserLoanOffers { get; set; } = default!;
}