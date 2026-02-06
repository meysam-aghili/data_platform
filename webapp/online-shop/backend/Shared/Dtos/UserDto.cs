using System.Text.Json.Serialization;

namespace Shared.Dtos;

public record UserDto : BaseDto
{
    [JsonPropertyOrder(5)]
    public string Phone { get; set; } = string.Empty;

    [JsonPropertyOrder(6)]
    public ICollection<UserAddressDto>? UserAddresses { get; set; }
}
