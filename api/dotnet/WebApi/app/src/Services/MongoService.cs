using MongoDB.Driver;
using MongoDB.Bson;
using WebApi.Models;
using WebApi.Shared;


namespace WebApi.Services;

public class MongoService<T>(IConfiguration config, IHttpContextAccessor http) : IMongoService<T> where T : BaseBsonDocument
{
    private readonly string _mongoDb = config["MONGO_DB"] ?? throw new ConfigNotFoundException("MONGO_DB");
    private readonly MongoClient _client = new(config["MONGO_CONSTRING"] ?? throw new ConfigNotFoundException("MONGO_CONSTRING"));
    private IMongoCollection<T> Collection =>
        _client.GetDatabase(_mongoDb)
        .GetCollection<T>(GetCollectionName(typeof(T)));

    private static protected string GetCollectionName(Type documentType) =>
        Attribute.GetCustomAttribute(documentType, typeof(BsonCollectionAttribute))
        is not BsonCollectionAttribute collection
        ? throw new InvalidCollectionException("Invalid collection name.")
        : collection.CollectionName;

    public IQueryable<T> Query() => Collection.AsQueryable();

    public async Task<(IReadOnlyList<T> items, int count)> GetAsync(
        string? createdBy = null,
        string? query = null,
        int page = 0,
        int pageSize = 0)
    {
        IQueryable<T>? itemsQuery =
            (from item in this.Query()
            orderby item.UpdatedAt descending, item.CreatedAt descending
            select item) as IQueryable<T>;

        if (!string.IsNullOrWhiteSpace(query))
        {
            itemsQuery =
                from item in itemsQuery
                where (item.Slug ?? string.Empty).Contains(query)
                select item;
        }

        if (!string.IsNullOrWhiteSpace(createdBy))
        {
            itemsQuery =
                from item in itemsQuery
                where item.CreatedBy == createdBy
                select item;
        }

        itemsQuery = pageSize switch
        {
            0 => itemsQuery,
            > 0 => itemsQuery.Skip(pageSize * page).Take(pageSize),
            _ => throw new ArgumentException(null, nameof(pageSize))
        };

        Task<List<T>> itemsTask = Task.FromResult(itemsQuery.ToList());
        Task<int> countTask = Task.FromResult(itemsQuery.Count());

        await Task.WhenAll(itemsTask, countTask);

        IReadOnlyList<T> items = (await itemsTask).AsReadOnly();
        int count = await countTask;
        return (items, count);
    }

    //public async Task DeleteAsync(string id) =>
    //    await Collection.DeleteOneAsync(Builders<T>.Filter.Eq("_id", ObjectId.Parse(id)));

    //public async Task DeleteAsync(T entity) =>
    //    await Collection.DeleteOneAsync(e => e.Id == entity.Id);

    //public async Task DeleteManyAsync(IEnumerable<string> ids) =>
    //    await Collection.DeleteManyAsync(Builders<T>.Filter.AnyIn("Id", ids));

    //public async Task<List<T>> GetAllAsync() =>
    //    await (await Collection.FindAsync(Builders<T>.Filter.Empty)).ToListAsync();

    //public async Task<T> GetAsync(string id) =>
    //    await (await Collection.FindAsync(Builders<T>.Filter.Eq("_id", ObjectId.Parse(id)))).FirstOrDefaultAsync();

    //public async Task<List<T>> GetManyAsync(IEnumerable<string> ids) =>
    //    await (await Collection.FindAsync(Builders<T>.Filter.AnyIn("Id", ids))).ToListAsync();

    //public async Task CreateAsync(T document)
    //{
    //    document.CreatedAt = DateTime.Now;
    //    document.CreatedBy = http.HttpContext?.User?.Identity?.Name?.ToLower();
    //    await Collection.InsertOneAsync(document);
    //}

    //// Fix this (like above) to include the created-at and created-by.
    //public async Task CreateManyAsync(IEnumerable<T> documents) =>
    //    await Collection.InsertManyAsync(documents);

    //public async Task UpdateAsync(T document)
    //{
    //    document.UpdatedAt = DateTime.Now;
    //    document.UpdatedBy = http.HttpContext?.User?.Identity?.Name?.ToLower();
    //    await Collection.ReplaceOneAsync(Builders<T>.Filter.Eq("Id", document.Id), document);
    //}

    //public async Task PutAsync(T document)
    //{
    //    if (await ExistsAsync(document.Slug))
    //    {
    //        await UpdateAsync(document);
    //    }
    //    else
    //    {
    //        await CreateAsync(document);
    //    }
    //}

    //public async Task<bool> ExistsAsync(string slug) =>
    //    (await Collection.FindAsync(Builders<T>.Filter.Eq("slug", slug))).Any();

    //public List<T> GetAll() =>
    //    Collection.Find(Builders<T>.Filter.Empty).ToList();

    //public async Task<T?> GetBySlugAsync(string slug) =>
    //    await (await Collection.FindAsync(Builders<T>.Filter.Eq("slug", slug))).FirstOrDefaultAsync();
}

public class InvalidCollectionException : Exception
{
    public InvalidCollectionException()
    {
    }
    public InvalidCollectionException(string? message) : base(message)
    {
    }
    public InvalidCollectionException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
