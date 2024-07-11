using System.ComponentModel.DataAnnotations;

namespace App.DTO.v1_0.Identity;

public class AppRefreshToken
{
    public Guid Id { get; set; }

    public Guid AppUserId { get; set; } = default!;

    [MaxLength(64)] public string RefreshToken { get; set; } = default!;
    public DateTime ExpirationDT  { get; set; }
    

    [MaxLength(64)]
    public string? PreviousRefreshToken { get; set; } 
    public DateTime PreviousExpirationDT  { get; set; }
}