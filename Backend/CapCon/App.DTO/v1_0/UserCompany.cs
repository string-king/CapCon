

using App.DTO.v1_0.Identity;

namespace App.DTO.v1_0;

public class UserCompany
{
    public Guid Id { get; set; }
    
    public Guid AppUserId { get; set; }
    public AppUser AppUser { get; set; } = default!;
    
    public Guid CompanyId { get; set; }
    public Company Company { get; set; } = default!;

    public Enums.ECompanyRole Role { get; set; }
}

public class UserCompanyForCompany
{
    public Guid Id { get; set; }
    public Guid? CompanyId { get; set; }
    public Guid AppUserId { get; set; }
    public AppUser AppUser { get; set; } = default!;
    public Enums.ECompanyRole Role { get; set; }

}

