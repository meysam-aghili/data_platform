using System.ComponentModel.DataAnnotations;

namespace Shared.Models;

public class Image: Base
{
    [Required]
    public string Url { get; set; } = string.Empty;

    [Required]
    public int SortNumber { get; set; }
}
