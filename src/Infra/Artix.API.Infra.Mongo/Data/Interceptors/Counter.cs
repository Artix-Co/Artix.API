namespace Artix.API.Infra.Mongo.Data.Interceptors;

using MongoDB.Bson.Serialization.Attributes;

public class Counter
{
    [BsonId]
    public string Id { get; set; } // Collection name (e.g., "users")

    [BsonElement("sequence_value")] // Map to snake_case field in MongoDB
    public long SequenceValue { get; set; } // Current sequence value
}
