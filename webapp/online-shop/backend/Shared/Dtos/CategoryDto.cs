using System.Text.Json.Serialization;

namespace Shared.Dtos;

public record CategoryDto: BaseDto
{
    [JsonPropertyOrder(4)]
    public List<CategoryDto> Children { get; set; } = [];
}
