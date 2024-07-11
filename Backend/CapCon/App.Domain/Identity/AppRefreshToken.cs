using Base.Contracts.Domain;
using Base.Domain;

namespace App.Domain.Identity;

public class AppRefreshToken : BaseRefreshToken, IDomainEntityId
{
    public Guid AppUserId { get; set; }
    public AppUser? AppUser { get; set; }
}