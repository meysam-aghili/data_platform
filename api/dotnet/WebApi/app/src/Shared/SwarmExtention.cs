using System.ComponentModel;
using WebApi.Models;


namespace WebApi.Shared;

public static class MapperExtensions
{
    public static SwarmContainerModel ToContainer(this SwarmJobBsonDocument job) => new()
    {
        Name = job.Slug,
        CreatedBy = null,
        Namespace = "api",
        Image = job.Image,
        Entrypoint = job.Entrypoint,
        Command = job.Command,
        Environment = job.Environment,
        Secrets = job.Secrets,
        Networks = job.Networks,
        UseHostNetwork = job.UseHostNetwork,
        Detached = job.Detached
    };
}