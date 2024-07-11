using System.ComponentModel.DataAnnotations;
using Base.Contracts.Domain;

namespace App.BLL.DTO.Identity;

public class AppRefreshToken : IDomainEntityId
{
    public Guid Id { get; set; }

    public Guid AppUserId { get; set; } = default!;

    [MaxLength(64)] public string RefreshToken { get; set; } = default!;
    public DateTime ExpirationDT  { get; set; }


    [MaxLength(64)] public string? PreviousRefreshToken { get; set; }
    public DateTime PreviousExpirationDT  { get; set; } 
}