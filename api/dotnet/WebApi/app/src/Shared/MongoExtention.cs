using MongoDB.Bson;
using System.Text.Json.Serialization;
using System.Text.Json;


namespace WebApi.Shared;

public class ObjectIdConverter : JsonConverter<ObjectId>
{
    public override void Write(Utf8JsonWriter writer, ObjectId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }

    public override ObjectId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string value = reader.GetString();
        return new ObjectId(value);
    }
}

