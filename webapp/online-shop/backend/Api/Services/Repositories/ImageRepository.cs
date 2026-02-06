using Api.Shared;
using Minio;

namespace Api.Services.Repositories;

public class ImageRepository
{
    private readonly IMinioClient _client;
    private readonly string _bucket;
    private readonly int _expiry;
    public ImageRepository(IConfiguration config)
    {
        _client = new MinioClient()
            .WithEndpoint(config.GetConfig("MINIO_ENDPOINT"), int.Parse(config.GetConfig("MINIO_PORT")))
            .WithCredentials(config.GetConfig("MINIO_ACCESS_KEY"), config.GetConfig("MINIO_SECRET_KEY"))
            .Build();
        _bucket = config.GetConfig("MINIO_BUCKET");
        _expiry = int.Parse(config.GetConfig("MINIO_EXPIRY"));
    }
    
    public async Task<string> GetUrl(string @object)
    {
        return await _client.PresignedGetObjectAsync(
            new Minio.DataModel.Args.PresignedGetObjectArgs()
            .WithBucket(_bucket)
            .WithObject(@object)
            .WithExpiry(_expiry)
        );
    }
}
