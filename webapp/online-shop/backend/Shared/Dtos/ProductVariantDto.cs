using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Shared.Dtos;

public record ProductVariantDto : BaseDto
{
    [JsonPropertyOrder(4)]
    public float Price { get; set; }

    [JsonPropertyOrder(5)]
    public string ColorTitle { get; set; } = "Unknown";

    [JsonPropertyOrder(6)]
    public string SizeTitle { get; set; } = "Unknown";

    [JsonPropertyOrder(7)]
    public int Stock { get; set; }

    [JsonPropertyOrder(8)]
    public float Discount { get; set; }

    [JsonPropertyOrder(9)]
    public string? Description { get; set; } = string.Empty;

    [JsonPropertyOrder(10)]
    public int SortNumber { get; set; }

    [JsonPropertyOrder(11)]
    public ICollection<ImageDto>? Images { get; set; }
}
