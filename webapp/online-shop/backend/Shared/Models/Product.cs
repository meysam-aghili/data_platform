using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models;

public class Product: Base
{
    public string? Description { get; set; } = string.Empty;

    [Required]
    public long BrandId { get; set; }

    [ForeignKey(nameof(BrandId))]
    public Brand Brand { get; set; } = null!;

    [Required]
    public long CategoryId { get; set; }

    [ForeignKey(nameof(CategoryId))]
    public Category Category { get; set; } = null!;

    public ICollection<ProductVariant>? ProductVariants { get; set; }
}
