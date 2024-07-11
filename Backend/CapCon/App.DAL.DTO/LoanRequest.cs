using System.ComponentModel.DataAnnotations;
using Base.Contracts.Domain;

namespace App.DAL.DTO;

public class LoanRequest : IDomainEntityId
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public Company? Company { get; set; }
    
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