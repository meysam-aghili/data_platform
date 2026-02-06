using System.Text.Json.Serialization;

namespace Shared.Dtos;

public record BaseDto
{
    [JsonPropertyOrder(1)]
    public long Id { get; set; }

    [JsonPropertyOrder(2)]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyOrder(3)]
    public string Slug { get; set; } = string.Empty;
}
