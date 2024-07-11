using System.ComponentModel.DataAnnotations;
using App.DAL.DTO.Identity;
using Base.Contracts.Domain;

namespace App.DAL.DTO;

public class Loan : IDomainEntityId
{
    public Guid Id { get; set; }
    
    public Guid CompanyId { get; set; }
    public Company? Company { get; set; }
    
    public Guid AppUserId { get; set; }
    public AppUser? AppUser { get; set; }
    public bool Active { get; set; }
    
    public ICollection<Transaction>? Transactions { get; set; }
    
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be larger than 0")]
    public decimal Amount { get; set; }
    
    [Range(0.01, double.MaxValue, ErrorMessage = "Interest must be larger than 0")]
    public decimal Interest { get; set; }
    
    [Range(0, Int32.MaxValue, ErrorMessage = "Period must be positive integer.")]
    public int Period { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [MaxLength(500)]
    public string? Comment { get; set; }
}