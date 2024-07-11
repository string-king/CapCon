using System.ComponentModel.DataAnnotations;

namespace Base.Domain;

public class BaseRefreshToken : BaseRefreshToken<Guid>
{
    
}

public class BaseRefreshToken<TKey> : BaseEntityId<TKey>
where TKey : IEquatable<TKey>
{
    [MaxLength(64)]
    public string RefreshToken { get; set; } = Guid.NewGuid().ToString();
    public DateTime ExpirationDT { get; set; } = DateTime.UtcNow.AddDays(7);
    
    [MaxLength(64)]
    public string? PreviousRefreshToken { get; set; } 
    public DateTime? PreviousExpirationDT { get; set; } 
}