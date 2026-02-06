using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models;

public class ProductVariant: Base
{
    [Required]
    public float Price { get; set; }

    public long ColorId { get; set; }
    
    public long SizeId { get; set; }

    public string? Description { get; set; } = string.Empty;

    [Required]
    public int SortNumber { get; set; }

    [Required]
    public int Stock { get; set; }

    public float Discount { get; set; }

    [ForeignKey(nameof(ColorId))]
    public Color Color { get; set; } = null!;

    [ForeignKey(nameof(SizeId))]
    public Size Size { get; set; } = null!;

    public ICollection<Image>? Images { get; set; }
}
