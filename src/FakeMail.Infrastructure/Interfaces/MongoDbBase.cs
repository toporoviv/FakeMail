using FakeMail.Repositories.Options;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace FakeMail.Repositories.Interfaces;

public abstract class MongoDbBase(IOptions<MongoDbSettings> mongoDbSettings)
{
    private readonly MongoDbSettings _mongoDbSettings = mongoDbSettings.Value;
    
    protected IMongoDatabase GetDatabase(string dbName)
    {
        var connectionString = GetConnectionString();
        
        return new MongoClient(connectionString).GetDatabase(dbName);
    }

    private string GetConnectionString() =>
        $"mongodb://{_mongoDbSettings.User}:{_mongoDbSettings.Password}@mongo:{_mongoDbSettings.Port}";
}