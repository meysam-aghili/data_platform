using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace BlazorApp.Models;

public class SqlStoredProcedureModel
{
    [JsonPropertyName("database")]
    [BsonElement("database")]
    public SqlDatabaseModel Database { get; set; } = null!;

    [JsonPropertyName("schema")]
    [BsonElement("schema")]
    public string Schema { get; set; } = null!;

    [JsonPropertyName("stored_procedure")]
    [BsonElement("stored_procedure")]
    public string StoredProcedure { get; set; } = null!;
}

public class SqlDatabaseModel
{
    [BsonElement("server")]
    public string Server { get; set; } = null!;

    [BsonElement("database")]
    public string? Database { get; set; }
}

public class SqlCredentialModel
{
    public string Username { get; set; } = null!;
    public string? Password { get; set; }
}

public class SqlOptionModel
{
    public SqlDatabaseModel? Database { get; set; }
    public SqlCredentialModel? Credentials { get; set; }
    public int? PoolSize { get; set; } = 50;
    public int CommandTimeoutSeconds { get; set; } = 30;
    public int Port { get; set; } = 1433;

    public void Validate()
    {
        ArgumentException.ThrowIfNullOrEmpty(Credentials.Username, $"{nameof(Credentials)}.{nameof(Credentials.Username)}");
        ArgumentException.ThrowIfNullOrEmpty(Credentials.Password, $"{nameof(Credentials)}.{nameof(Credentials.Password)}");
        ArgumentException.ThrowIfNullOrEmpty(Database.Server, $"{nameof(Database)}.{nameof(Database.Server)}");

        if (Port is < 1 or > 65535)
            throw new ArgumentOutOfRangeException(nameof(Port), "Must be between 1-65535");
    }

    public static SqlOptionModel operator |(SqlOptionModel? left, SqlOptionModel? right)
    {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(right);

        SqlOptionModel result = new()
        {
            Database = new SqlDatabaseModel
            {
                Server = left.Database?.Server ?? right.Database?.Server
                    ?? throw new ArgumentNullException(nameof(left), "Database Server is missing and no default found."),
                Database = left.Database?.Database ?? right.Database?.Database ?? string.Empty
            },
            Credentials = new SqlCredentialModel
            {
                Username = left.Credentials?.Username ?? right.Credentials?.Username
                    ?? throw new ArgumentNullException(nameof(left), "Username is missing and no default found."),
                Password = left.Credentials?.Password ?? right.Credentials?.Password
                    ?? throw new ArgumentNullException(nameof(left), "Password is missing and no default found.")
            },
            PoolSize = left.PoolSize ?? right.PoolSize ?? null,
            CommandTimeoutSeconds = left.CommandTimeoutSeconds,
            Port = left.Port > 0 ? left.Port : right.Port
        };
        return result;
    }
}

public enum SqlUser
{
    SqlApiUser,
    SqlApiUnmaskedUser

}

public class SqlStoredProcedureParameterModel : ICloneable
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    public object? Value { get; set; }

    [JsonPropertyName("max_length")]
    public int MaxLength { get; set; }

    [JsonPropertyName("is_output")]
    public bool IsOutput { get; set; }

    [JsonIgnore]
    public bool IsInput => !IsOutput;

    public object Clone() => MemberwiseClone();
}

public class SqlStoredProcedureResultModel
{
    public List<List<dynamic>> ResultSets { get; set; } = [];
    public List<SqlStoredProcedureParameterModel>? OutputParameters { get; set; }
}

public enum ApiSqlServer
{
    Warehouse
}