namespace Api.Shared;

public static class Helpers
{
    public static string GetConfig(this IConfiguration config, string name) => config[name] ?? throw new InvalidOperationException($"config not found: {name}");
}
