using System.Linq.Expressions;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using StockChat.Api.Models;

namespace StockChat.Api.Data;

public class Repository<T> : IRepository<T> where T : IEntity
{
    private readonly IMongoDatabase _mongoDb;
    
    public Repository(IOptions<MongoOptions> options)
    {
        _mongoDb = new MongoClient(options.Value.Connection).GetDatabase(options.Value.Database);
    }

    public Task<List<T>> GetAll()
    {
        return GetFiltered(x => true);
    }

    public async Task<List<T>> GetFiltered(Expression<Func<T, bool>> filter)
    {
        var collection = await GetOrCreateCollectionAsync();
        var rawData = await collection.FindAsync(filter);
        var result = await rawData.ToListAsync();
        return result;
    }

    public async Task<string> Register(T entity)
    {
        entity.Id = ObjectId.GenerateNewId().ToString();
        var collection = await GetOrCreateCollectionAsync();
        await collection.InsertOneAsync(entity);

        return entity.Id;
    }
    
    private async Task<IMongoCollection<T>> GetOrCreateCollectionAsync()
    {
        var exists = await CollectionExistsAsync(typeof(T).Name);
        
        if (exists) return _mongoDb.GetCollection<T>(typeof(T).Name);
        await _mongoDb.CreateCollectionAsync(typeof(T).Name);
        return _mongoDb.GetCollection<T>(typeof(T).Name);
    }
    
    private async Task<bool> CollectionExistsAsync(string collectionName)
    {
        var filter = new BsonDocument("name", collectionName);
        var collections = await _mongoDb.ListCollectionsAsync(new ListCollectionsOptions { Filter = filter });
        return await collections.AnyAsync();
    }
}