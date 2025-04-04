using Nest;


namespace BlazorApp.Services.Elastic;

public interface IElasticService
{
    new Task<T> GetAsync<T>(string index, string id) where T : class;
    new Task<IEnumerable<T>> GetManyAsync<T>(string index, IEnumerable<string> ids) where T : class;
    Task<IEnumerable<T>> GetAllAsync<T>(string index, int from, int size) where T : class;
    new Task IndexAsync<T>(string index, T document) where T : class;
    new Task IndexManyAsync<T>(string index, IEnumerable<T> documents) where T : class;
    Task UpdateAsync<T>(string index, Guid id, T partialDocument) where T : class;
    Task<IEnumerable<T>> SearchAsync<T>(string index, Func<SearchDescriptor<T>, ISearchRequest> request) where T : class;
    Task DeleteAsync<T>(string index, T document) where T : class;
    Task DeleteManyAsync<T>(string index, IEnumerable<T> documents) where T : class;
    Task DeleteAsync(string index, Guid id);
    new Task DeleteAsync(string index, string id);
    Task<T> GetAsync<T>(string index, Guid id) where T : class;
    Task<IEnumerable<T>> GetManyAsync<T>(string index, IEnumerable<Guid> ids) where T : class;
    List<T> GetAll<T>(string index) where T : class;
    new Task<List<T>> GetAllAsync<T>(string index) where T : class;
    Task<bool> Exists(string index, string id);
    Task<bool> Exists(string index, Guid id);
    public Task<List<Dictionary<string, object>>> QuerySqlAsync(string query);
}
