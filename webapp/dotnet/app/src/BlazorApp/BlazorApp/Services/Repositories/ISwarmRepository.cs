using Microsoft.AspNetCore.DataProtection;
using BlazorApp.Services;
using BlazorApp.Models;
using BlazorApp.Shared;
using System.ComponentModel;


namespace BlazorApp.Services;

public interface ISwarmRepository
{
    string Key { get; set; }

    //Task<List<ConfigDto>> GetConfigsAsync(SwarmObjectQuery? query);
    //Task<string> CreateConfigAsync(Config config);
    //Task DeleteConfigAsync(string id);

    //Task<List<SecretDto>> GetSecretsAsync(SwarmObjectQuery? query);
    //Task<string> CreateSecretAsync(Secret secret);
    //Task DeleteSecretAsync(string id);

    //Task<List<ServiceDto>> GetServicesAsync(SwarmObjectQuery? query);
    //Task<string> CreateServiceAsync(Service service);
    //Task DeleteServiceAsync(string id);

    Task<ResultModel> RunContainerAsync(SwarmContainerModel container);
}
