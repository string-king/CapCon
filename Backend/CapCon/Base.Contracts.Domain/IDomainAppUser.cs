using Microsoft.AspNetCore.Identity;

namespace Base.Contracts.Domain;

public interface IDomainAppUser<TUser> : IDomainAppUser<Guid, TUser>, IDomainAppUserId
    where TUser: IdentityUser<Guid>
{
    
}

public interface IDomainAppUser<TKey, TUser>: IDomainAppUserId<TKey> 
    where TKey : IEquatable<TKey>
    where TUser: IdentityUser<TKey>
{
    public TUser? AppUser { get; set; }
}