namespace App.DTO.v1_0.Identity;

public class AppUser
{
    public Guid Id { get; set; }

    public string Email { get; set; } = default!;
    
    public string FirstName { get; set; } = default!;
    
    public string LastName { get; set; } = default!;
}