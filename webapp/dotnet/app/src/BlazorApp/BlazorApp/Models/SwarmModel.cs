using System.Text.Json.Serialization;
using BlazorApp.Shared;
using System.ComponentModel.DataAnnotations;
using BlazorApp.Models;


namespace BlazorApp.Models
{
    public class SwarmObjectModel
    {
        [JsonPropertyName("id")]
        [Display(Name = "ID")]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        [Display(Name = "Name")]
        public string Name { get; set; } = null!;

        [JsonIgnore]
        public LdapUserModel? CreatedBy { get; set; }

        [JsonIgnore]
        [Display(Name = "namespace")]
        public string Namespace { get; set; } = string.Empty;

        [JsonIgnore]
        public Dictionary<string, string> CustomLabels = [];

        [JsonPropertyName("labels")]
        [Display(Name = "Namespace")]
        public Dictionary<string, string> Labels => CustomLabels.AddRange(new()
    {
        { "com.digikala.biapps.created.by", CreatedBy?.Id.ToLower() ?? string.Empty },
        { "com.digikala.biapps.namespace", Namespace }
    });
    }
}

public class SwarmContainerModel : SwarmObjectModel
{
    [JsonPropertyName("image")]
    [Display(Name = "Image", Description = "Image along with the tag.")]
    public string Image { get; set; } = null!;

    [JsonPropertyName("entrypoint")]
    [Display(Name = "Entrypoint", Description = "Task ENTRYPOINT.")]
    public string? Entrypoint { get; set; } = null;

    [JsonPropertyName("command")]
    [Display(Name = "Command", Description = "Task CMD.")]
    public string? Command { get; set; } = null;

    [JsonPropertyName("environment")]
    [Display(Name = "Environment", Description = "Environment variables to pass to the service.")]
    public Dictionary<string, string> Environment { get; set; } = [];

    [JsonPropertyName("secrets")]
    [Display(Name = "Secrets", Description = "The secrets to inject to the service.")]
    public Dictionary<string, string> Secrets { get; set; } = [];

    [JsonPropertyName("networks")]
    [Display(Name = "Networks", Description = "Networks to attach.")]
    public List<string>? Networks { get; set; } = null;

    [JsonPropertyName("host_network")]
    [Display(Name = "Use host network")]
    public bool UseHostNetwork { get; set; } = false;

    [JsonPropertyName("detached")]
    [Display(Name = "Start the container in background.")]
    public bool Detached { get; set; } = false;
}