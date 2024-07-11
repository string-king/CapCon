using App.DTO.v1_0.Identity;

namespace App.DTO.v1_0;

public class Loan
{
    public Guid Id { get; set; }
    
    public Guid CompanyId { get; set; }
    public CompanySimple Company { get; set; } = default!;
    
    public Guid AppUserId { get; set; }
    public AppUser AppUser { get; set; } = default!;
    public bool Active { get; set; }
    
    
    public ICollection<Transaction>? Transactions { get; set; }
    
    public decimal Amount { get; set; }
    
    public decimal Interest { get; set; }
    
    public int Period { get; set; }
    
    public DateTime StartDate { get; set; }
    
    public string? Comment { get; set; }
}