using System.ComponentModel.DataAnnotations;

namespace Shared.Models;

public class User : Base
{
    [Required]
    public string Phone {  get; set; } = string.Empty;
    
    [Required]
    public string Email { get; set; } = string.Empty;

    public ICollection<UserAddress>? UserAddresses { get; set; }
}
