using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos;

public class UserAddressCreateDto
{
    [Required]
    public string Address { get; set; } = string.Empty;

    [Required]
    public string PostalCode { get; set; } = string.Empty;

    [Required]
    public string StateTitle { get; set; } = string.Empty;

    [Required]
    public string CityTitle { get; set; } = string.Empty;

    [Required]
    public string Title { get; set; } = string.Empty;
}
