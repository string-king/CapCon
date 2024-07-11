using App.Domain.Enums;
using App.Domain.Identity;
using Base.Contracts.Domain;
using Base.Domain;

namespace App.Domain;

public class UserCompany : BaseEntityId, IDomainAppUser<AppUser>
{
    public Guid AppUserId { get; set; }
    public AppUser? AppUser { get; set; }
    
    public Guid CompanyId { get; set; }
    public Company? Company { get; set; }
    
    public ECompanyRole Role { get; set; }
}