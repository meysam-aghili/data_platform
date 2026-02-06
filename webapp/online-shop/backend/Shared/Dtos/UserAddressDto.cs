using System.Text.Json.Serialization;

namespace Shared.Dtos;

public record UserAddressDto : BaseDto
{
    [JsonPropertyOrder(5)]
    public string Address { get; set; } = string.Empty;

    [JsonPropertyOrder(6)]
    public string PostalCode { get; set; } = string.Empty;

    [JsonPropertyOrder(7)]
    public string StateTitle { get; set; } = string.Empty;

    [JsonPropertyOrder(8)]
    public string CityTitle { get; set; } = string.Empty;
}
