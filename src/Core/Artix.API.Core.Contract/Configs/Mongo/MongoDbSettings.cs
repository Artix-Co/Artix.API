namespace Artix.API.Core.Contract.Configs.Mongo;

public class MongoDbSettings
{
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
}
