using System.ComponentModel.DataAnnotations;
using App.BLL.DTO.Enums;
using Base.Contracts.Domain;

namespace App.BLL.DTO;

public class Transaction : IDomainEntityId
{
    public Guid Id { get; set; }
    public Guid LoanId { get; set; } 
    public Loan? Loan { get; set; }
    public ETransactionType TransactionType { get; set; }
    [DataType(DataType.Date)]
    public DateTime Date { get; set; }
    [Range(0, Double.MaxValue, ErrorMessage = "Amount must be positive decimal number!")]
    public decimal Amount { get; set; }
    public string? Comment { get; set; }
}