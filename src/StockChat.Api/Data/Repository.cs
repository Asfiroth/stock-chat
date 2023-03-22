using System.Linq.Expressions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StockChat.Api.Models;

namespace StockChat.Api.Data;

public class Repository<T> : IRepository<T>
{
    private readonly IMongoDatabase _mongoDb;
    
    public Repository(IOptions<MongoOptions> options)
    {
        _mongoDb = new MongoClient(options.Value.Connection).GetDatabase(options.Value.Database);
    }

    public async Task<List<T>> GetFiltered(Expression<Func<T, bool>> filter)
    {
        var collection = await GetOrCreateCollectionAsync();
        var rawData = await collection.FindAsync(filter);
        var result = await rawData.ToListAsync();
        return result;
    }

    public async Task Register(T entity)
    {
        var collection = await GetOrCreateCollectionAsync();
        await collection.InsertOneAsync(entity);
    }
    
    private async Task<IMongoCollection<T>> GetOrCreateCollectionAsync()
    {
        var collection = _mongoDb.GetCollection<T>(typeof(T).Name);
        if (collection != null) return collection;
        await _mongoDb.CreateCollectionAsync(typeof(T).Name);
        collection = _mongoDb.GetCollection<T>(typeof(T).Name);
        return collection;
    }
}