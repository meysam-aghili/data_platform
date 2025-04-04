using MongoDB.Bson;
using WebApi.Models;


namespace WebApi.Services;

public interface IMongoService<T> where T : BaseBsonDocument
{
    //public Task DeleteAsync(string id);
    //public Task DeleteAsync(T entity);
    //public Task DeleteManyAsync(IEnumerable<string> ids);
    //public Task<List<T>> GetAllAsync();
    //public List<T> GetAll();
    //public Task<T> GetAsync(string id);
    //public Task<T?> GetBySlugAsync(string slug);
    //public Task<List<T>> GetManyAsync(IEnumerable<string> ids);
    //public Task CreateAsync(T entity);
    //public Task CreateManyAsync(IEnumerable<T> entities);
    //public Task UpdateAsync(T entity);
    //public Task<bool> ExistsAsync(string slug);
    //public Task PutAsync(T document);
    public IQueryable<T> Query();
    public Task<(IReadOnlyList<T> items, int count)> GetAsync(
        string? createdBy = null,
        string? query = null,
        int page = 0,
        int pageSize = 0);
}
