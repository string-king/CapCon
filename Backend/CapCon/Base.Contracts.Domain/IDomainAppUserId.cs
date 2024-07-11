namespace Base.Contracts.Domain;

public interface IDomainAppUserId : IDomainAppUserId<Guid>
{
    
}

public interface IDomainAppUserId<TKey>
    where TKey : IEquatable<TKey>
{
    public TKey AppUserId { get; set; }
}