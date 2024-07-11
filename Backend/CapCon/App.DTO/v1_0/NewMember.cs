using App.DTO.v1_0.Enums;

namespace App.DTO.v1_0;

public class NewMember
{
    public Guid CompanyId { get; set; }
    public string Email { get; set; } = default!;
    public ECompanyRole Role { get; set; }
}