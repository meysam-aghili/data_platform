using BlazorApp.Shared;
using Nest;


namespace BlazorApp.Services.Elastic;

public class ElasticService : IElasticService
{
    private readonly string _host;
    private readonly string _username;
    private readonly string _password;
    private readonly ConnectionSettings _settings;
    private ElasticClient _client;

    public ElasticService(IConfiguration config)
    {
        _host = config["ELASTICSEARCH_ADDRESS"] ?? throw new ConfigNotFoundException("ELASTICSEARCH_ADDRESS");
        _username = config["ELASTICSEARCH_USERNAME"] ?? throw new ConfigNotFoundException("ELASTICSEARCH_USERNAME");
        _password = config["ELASTICSEARCH_PASSWORD"] ?? throw new ConfigNotFoundException("ELASTICSEARCH_PASSWORD");
        _settings = new ConnectionSettings(new Uri(_host));
        _settings.BasicAuthentication(_username, _password);
        _settings.DisableDirectStreaming();
        _client = new ElasticClient(_settings);
    }

    private void ReIndex(string index)
    {
        _settings.DefaultIndex(index);
        _client = new ElasticClient(_settings);
    }

    public async Task<T> GetAsync<T>(string index, string id) where T : class
    {
        ReIndex(index);
        var response = await _client.GetAsync<T>(id);
        return response.Source;
    }

    public async Task<T> GetAsync<T>(string index, Guid id) where T : class
    {
        ReIndex(index);
        var response = await _client.GetAsync<T>(id);
        return response.Source;
    }

    public async Task<IEnumerable<T>> GetAllAsync<T>(string index, int from = 0, int size = 10000) where T : class
    {
        ReIndex(index);
        var searchResponse = await _client.SearchAsync<T>(s => s
            .Query(q => q
                .MatchAll())
            .From(from)
            .Size(size)
        );
        return searchResponse.Documents;
    }

    public async Task<IEnumerable<T>> GetManyAsync<T>(string index, IEnumerable<string> ids) where T : class
    {
        ReIndex(index);
        var response = await _client.MultiGetAsync(s => s.GetMany<T>(ids, (gs, i) => gs.Index(index)));
        return response.SourceMany<T>(ids);
    }

    public async Task<IEnumerable<T>> GetManyAsync<T>(string index, IEnumerable<Guid> ids) where T : class
    {
        ReIndex(index);
        var response = await _client.MultiGetAsync(s => s.GetMany<T>(ids.Select(id => id.ToString()), (gs, i) => gs.Index(index)));
        return response.SourceMany<T>(ids.Select(id => id.ToString()));
    }

    public async Task IndexAsync<T>(string index, T document) where T : class
    {
        ReIndex(index);
        var indexResponse = await _client.IndexDocumentAsync(document);
        await RefreshAsync();
    }

    public async Task IndexManyAsync<T>(string index, IEnumerable<T> documents) where T : class
    {
        ReIndex(index);
        var indexResponse = await _client.IndexManyAsync(documents);
        await RefreshAsync();
    }

    public async Task UpdateAsync<T>(string index, Guid id, T partialDocument) where T : class
    {
        ReIndex(index);
        var updateResponse = await _client.UpdateAsync<T>(id, u => u.Doc(partialDocument));
        await RefreshAsync();
    }

    public async Task<IEnumerable<T>> SearchAsync<T>(string index, Func<SearchDescriptor<T>, ISearchRequest> request) where T : class
    {
        ReIndex(index);
        var searchResponse = await _client.SearchAsync(request);
        return searchResponse.Documents;
    }

    public async Task DeleteAsync<T>(string index, T document) where T : class
    {
        ReIndex(index);
        await _client.DeleteAsync<T>(document);
        await RefreshAsync();
        return;
    }

    public async Task DeleteManyAsync<T>(string index, IEnumerable<T> documents) where T : class
    {
        ReIndex(index);
        await _client.DeleteManyAsync(documents);
        await RefreshAsync();
        return;
    }
    private async Task RefreshAsync()
    {
        await _client.Indices.RefreshAsync(Indices.All);
    }

    public List<T> GetAll<T>(string index) where T : class
    {
        ReIndex(index);
        var result = _client.Search<T>(s => s
            .Query(q => q
                .MatchAll())
            .From(0)
            .Size(10000)
        );
        return result.Documents.ToList();
    }

    public async Task<List<T>> GetAllAsync<T>(string index) where T : class
    {
        ReIndex(index);
        var result = await _client.SearchAsync<T>(s => s
            .Query(q => q
                .MatchAll())
            .From(0)
            .Size(10000)
        );
        return result.Documents.ToList();
    }

    public async Task DeleteAsync(string index, Guid id)
    {
        await _client.DeleteAsync(new DeleteRequest(index, id));
        await _client.Indices.RefreshAsync();
    }

    public async Task DeleteAsync(string index, string id)
    {
        await _client.DeleteAsync(new DeleteRequest(index, id));
        await _client.Indices.RefreshAsync();
    }

    public async Task<bool> Exists(string index, string id) =>
        (await _client.DocumentExistsAsync(new DocumentExistsRequest(index, id))).Exists;

    public async Task<bool> Exists(string index, Guid id) =>
        (await _client.DocumentExistsAsync(new DocumentExistsRequest(index, id))).Exists;

    public async Task<List<Dictionary<string, object>>> QuerySqlAsync(string query)
    {
        var response = await _client.Sql.QueryAsync(q => q.Query(query));
        var responseDict = new List<Dictionary<string, object>>();
        foreach (var row in response.Rows)
        {
            var rowDict = new Dictionary<string, object>();
            int index = 0;
            foreach (var col in response.Columns)
            {
                rowDict[col.Name] = row[index++].As<object>();
            }
            responseDict.Add(rowDict);
        }
        return responseDict;
    }
}
