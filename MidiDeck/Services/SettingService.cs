using System.Text.Json;
using MidiDeck.Services.Interfaces;

namespace MidiDeck.Services;

public class SettingService : ISettingsService
{
    private readonly ApplicationDataContainer dataContainer;

    public SettingService() 
    {
        dataContainer = ApplicationData.Current.LocalSettings;
    }

    public T? Get<T>(string key)
    {
        if(dataContainer.Values.TryGetValue(key, out object? value))
        {
            return JsonSerializer.Deserialize<T>(value as string);
        }
        return default;
    }

    public void Set<T>(string key, T value)
    {
        dataContainer.Values[key] = JsonSerializer.Serialize(value);
    }
}
