
using App.BLL.DTO.Enums;
using App.BLL.DTO.Identity;
using Base.Contracts.Domain;

namespace App.BLL.DTO;

public class UserCompany : IDomainEntityId
{
    public Guid Id { get; set; }

    public Guid AppUserId { get; set; }
    public AppUser? AppUser { get; set; }
    
    public Guid CompanyId { get; set; }
    public Company? Company { get; set; }
    
    public ECompanyRole Role { get; set; }
}