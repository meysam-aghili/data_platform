using Shared.Models;
using System.Text.Json.Serialization;

namespace Shared.Dtos;

public record ProductDto : BaseDto
{
    [JsonPropertyOrder(4)]
    public string BrandTitle { get; set; } = "Unknown";

    [JsonPropertyOrder(5)]
    public string? Description { get; set; } = string.Empty;

    [JsonPropertyOrder(6)]
    public CategoryDto Category { get; set; } = null!;

    [JsonPropertyOrder(7)]
    public ICollection<ProductVariantDto>? ProductVariants { get; set; }
}
