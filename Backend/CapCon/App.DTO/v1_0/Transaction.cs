using System.ComponentModel.DataAnnotations;


namespace App.DTO.v1_0;

public class Transaction
{
    public Guid Id { get; set; }
    
    public Guid LoanId { get; set; } 
    
    public Enums.ETransactionType TransactionType { get; set; }
    
    [Range(0, Double.MaxValue, ErrorMessage = "Amount must be positive decimal number!")]
    public decimal Amount { get; set; }
    
    public string? Comment { get; set; }
    public DateTime Date { get; set; }
}