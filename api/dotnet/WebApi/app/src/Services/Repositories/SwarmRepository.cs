using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Extensions;
using WebApi.Services;
using WebApi.Models;
using WebApi.Shared;
using System.ComponentModel;
using WebApi.Services.Repositories;
using Docker.DotNet;
using Docker.DotNet.Models;
using MongoDB.Driver;
using static System.Net.Mime.MediaTypeNames;


namespace WebApi.Services;

public class SwarmRepository(
    IConfiguration config
    ) : ISwarmRepository
{
    public required string Key { get; set; }

    //public async Task<List<ConfigDto>> GetConfigsAsync(SwarmObjectQuery? query = null)
    //{
    //    using var client = _http.CreateClient(Key);
    //    return await client.GetFromJsonAsync<List<ConfigDto>>($"configs?{BiQueryBuilder.GetQueryString(query)}") ?? [];
    //}

    //public async Task<string> CreateConfigAsync(Config config)
    //{
    //    using var client = _http.CreateClient(Key);
    //    var response = await client.PostAsJsonAsync("configs", config);
    //    response.EnsureSuccessStatusCode();
    //    var id = await response.Content.ReadAsStringAsync();
    //    return id;
    //}

    //public async Task DeleteConfigAsync(string id)
    //{
    //    using var client = _http.CreateClient(Key);
    //    var response = await client.DeleteAsync($"configs/{id}");
    //    response.EnsureSuccessStatusCode();
    //}

    //public async Task<List<SecretDto>> GetSecretsAsync(SwarmObjectQuery? query = null)
    //{
    //    using var client = _http.CreateClient(Key);
    //    return await client.GetFromJsonAsync<List<SecretDto>>($"secrets?{BiQueryBuilder.GetQueryString(query)}") ?? [];
    //}

    //public async Task<string> CreateSecretAsync(Secret secret)
    //{
    //    using var client = _http.CreateClient(Key);
    //    var response = await client.PostAsJsonAsync("secrets", secret);
    //    response.EnsureSuccessStatusCode();
    //    var id = await response.Content.ReadAsStringAsync();
    //    return id;
    //}

    //public async Task DeleteSecretAsync(string id)
    //{
    //    using var client = _http.CreateClient(Key);
    //    var response = await client.DeleteAsync($"secrets/{id}");
    //    response.EnsureSuccessStatusCode();
    //}

    //public async Task<List<ServiceDto>> GetServicesAsync(SwarmObjectQuery? query = null)
    //{
    //    using var client = _http.CreateClient(Key);
    //    return await client.GetFromJsonAsync<List<ServiceDto>>($"services?{BiQueryBuilder.GetQueryString(query)}") ?? [];
    //}

    //public async Task<string> CreateServiceAsync(Service service)
    //{
    //    using var client = _http.CreateClient(Key);
    //    var response = await client.PostAsJsonAsync("services", service);
    //    response.EnsureSuccessStatusCode();
    //    var id = await response.Content.ReadAsStringAsync();
    //    return id;
    //}

    //public async Task DeleteServiceAsync(string id)
    //{
    //    using var client = _http.CreateClient(Key);
    //    var response = await client.DeleteAsync($"services/{id}");
    //    response.EnsureSuccessStatusCode();
    //}

    public async Task<ResultModel> RunContainerAsync(SwarmContainerModel container)
    {
        //Secrets
        var envs = new List<string>();
        foreach (var e in container.Environment)
        {
            envs.Add($"{e.Key}={e.Value}");
        }

        DockerClient client = new DockerClientConfiguration(
            new Uri(config["DOCKER_SOCKET_URI"] ?? throw new ConfigNotFoundException("DOCKER_SOCKET_URI")))
             .CreateClient();
        var response = await client.Containers.CreateContainerAsync(new CreateContainerParameters()
        {
            Image = container.Image,
            Entrypoint = [container.Entrypoint],
            Cmd = [container.Command],
            NetworkingConfig = new NetworkingConfig()
            { EndpointsConfig = container.Networks?.ToDictionary(k => k, v =>
                new EndpointSettings
                {
                    NetworkID = v
                })
            },
            AttachStderr = false,
            AttachStdin = false,
            AttachStdout = false,
            Tty = false,
            HostConfig = new HostConfig()
            {
                NetworkMode = container.UseHostNetwork ? "Host" : null,
                AutoRemove = true
            },
            Env = envs,
            Name = container.Name,
            Labels = container.Labels,

        });
        string containerId = response.ID;
        await client.Containers.StartContainerAsync(containerId, new ContainerStartParameters());
        return ResultModel.Success(metadata: new() { { "containerId", containerId } });
    }
}
