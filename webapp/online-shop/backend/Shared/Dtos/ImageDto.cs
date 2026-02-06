using System.Text.Json.Serialization;

namespace Shared.Dtos;

public record ImageDto: BaseDto
{
    [JsonPropertyOrder(5)]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyOrder(6)]
    public int SortNumber { get; set; }
}
