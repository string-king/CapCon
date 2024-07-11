using System.ComponentModel.DataAnnotations;
using App.DTO.v1_0.Identity;

namespace App.DTO.v1_0;

public class LoanOffer
{
    public Guid Id { get; set; }
    public Guid AppUserId { get; set; }
    public AppUser? AppUser { get; set; }
    
    public Guid LoanRequestId { get; set; }
    public LoanRequestSimple? LoanRequest { get; set; } = default!;
    
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be larger than 0")]
    public decimal Amount { get; set; }
    
    [Range(0.01, double.MaxValue, ErrorMessage = "Interest must be larger than 0")]
    public decimal Interest { get; set; }
    
    [Range(0, Int32.MaxValue, ErrorMessage = "Period must be positive integer.")]
    public int Period { get; set; }
    
    public bool Active { get; set; }
    
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }

}