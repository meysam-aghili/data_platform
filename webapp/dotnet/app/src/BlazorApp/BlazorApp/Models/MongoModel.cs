using System.Text.Json.Serialization;
using BlazorApp.Shared;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Nest;


namespace BlazorApp.Models;

public abstract class BaseBsonDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [JsonConverter(typeof(ObjectIdConverter))]
    // [JsonIgnore]
    public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

    [BsonElement("slug")]
    [JsonPropertyName("slug")]
    public string? Slug { get; set; }

    [BsonElement("created_at")]
    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; set; } = DateTime.Now;

    [BsonElement("created_by")]
    [JsonPropertyName("created_by")]
    public string? CreatedBy { get; set; }

    [BsonElement("updated_at")]
    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [BsonElement("updated_by")]
    [JsonPropertyName("updated_by")]
    public string? UpdatedBy { get; set; }
}

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class BsonCollectionAttribute(string collectionName) : Attribute
{
    public string CollectionName { get; } = collectionName;
}

[BsonCollection("roles")]
public class RoleModel : BaseBsonDocument
{
    [BsonElement("users")]
    public List<string> Users { get; set; } = [];
    public bool ContainsUser(string username) => Users.Contains(username, StringComparer.OrdinalIgnoreCase);
}

[BsonCollection("apis")]
public class ApiBsonDocument : BaseBsonDocument
{
    [BsonElement("users")]
    [Keyword(Name = "users")]
    [JsonPropertyName("users")]
    public List<string> Users { get; set; } = null!;

    [BsonElement("stored_procedure")]
    [JsonPropertyName("stored_procedure")]
    public SqlStoredProcedureModel StoredProcedure { get; set; } = new();

    [BsonElement("encryption_key")]
    [Keyword(Name = "encryption_key")]
    [JsonPropertyName("encryption_key")]
    public string? EncryptionKey { get; set; } = null;

    [BsonElement("method")]
    [Keyword(Name = "method")]
    [JsonPropertyName("method")]
    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public ApiHttpMethod Method { get; set; } = ApiHttpMethod.GET;

    [BsonElement("unmasked")]
    [Keyword(Name = "unmasked")]
    [JsonPropertyName("unmasked")]
    public bool Unmasked { get; set; } = false;
}

[BsonCollection("jobs")]
public class SwarmJobBsonDocument : BaseBsonDocument
{
    [BsonElement("users")]
    public List<string> Users { get; set; } = [];

    [BsonElement("image")]
    public string Image { get; set; } = null!;

    [BsonElement("entrypoint")]
    public string? Entrypoint { get; set; } = null;

    [BsonElement("command")]
    public string? Command { get; set; } = null;

    [BsonElement("environment")]
    public Dictionary<string, string> Environment { get; set; } = [];

    [BsonElement("secrets")]
    public Dictionary<string, string> Secrets { get; set; } = [];

    [BsonElement("networks")]
    public List<string>? Networks { get; set; } = [];

    [BsonElement("use_host_network")]
    public bool UseHostNetwork { get; set; } = false;

    [BsonElement("detached")]
    public bool Detached { get; set; } = false;
}

[BsonCollection("otps")]
public class OtpBsonDocument : BaseBsonDocument
{
    [BsonElement("username")]
    public string Username { get; set; } = null!;

    [BsonElement("expires_at")]
    public DateTime ExpiresAt { get; set; }

    [BsonElement("otp")]
    public string OtpString { get; set; } = null!;

    [BsonElement("used")]
    public bool Used { get; set; } = false;

    public override string ToString() => OtpString;
    public bool IsValid => !(Used || DateTime.UtcNow >= ExpiresAt);
}
