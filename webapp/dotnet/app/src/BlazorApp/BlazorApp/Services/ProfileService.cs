using BlazorApp.Models;
using Blazored.LocalStorage;


namespace BlazorApp.Services;

public class ProfileService(ILocalStorageService localStore)
{
    private readonly ILocalStorageService _localStore = localStore;

    public async Task ToggleDarkMode()
    {
        var preferences = await GetPreferences() ?? new();
        var newPreferences = preferences with
        {
            DarkMode = !preferences.DarkMode
        };

        await _localStore.SetItemAsync("preferences", newPreferences);
    }

    public async Task SetPreference(Preferences preferences)
        => await _localStore.SetItemAsync("preferences", preferences);

    public async Task<Preferences?> GetPreferences() =>
        await _localStore.GetItemAsync<Preferences>("preferences");
}

public record Preferences
{
    public bool DarkMode { get; init; }
}