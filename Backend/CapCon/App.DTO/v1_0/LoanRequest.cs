using System.ComponentModel.DataAnnotations;

namespace App.DTO.v1_0;

public class LoanRequest
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public CompanySimple? Company { get; set; } = default!;
    
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be larger than 0")]
    public decimal Amount { get; set; }
    
    [Range(0.01, double.MaxValue, ErrorMessage = "Interest must be larger than 0")]
    public decimal Interest { get; set; }
    
    [Range(0, Int32.MaxValue, ErrorMessage = "Period must be longer positive integer.")]
    public int Period { get; set; }
    
    public ICollection<LoanOffer>? LoanOffers { get; set; }
    public bool Active { get; set; }
    public string? Comment { get; set; }

    [DataType(DataType.Date)]
    public DateTime CreatedAt { get; set; }
}

public class LoanRequestSimple
{
    public Guid Id { get; set; }
    
    public Guid CompanyId { get; set; }
    public CompanySimple? Company { get; set; } = default!;
    
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be larger than 0")]
    public decimal Amount { get; set; }
    
    [Range(0.01, double.MaxValue, ErrorMessage = "Interest must be larger than 0")]
    public decimal Interest { get; set; }
    
    [Range(0, Int32.MaxValue, ErrorMessage = "Period must be longer positive integer.")]
    public int Period { get; set; }
    
    public bool Active { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}