using MidiDeck.Services.Interfaces;

namespace MidiDeck.Services;
public class SettingService : ISettingsService
{
    private readonly ApplicationDataContainer dataContainer;

    public SettingService() 
    {
        dataContainer = ApplicationData.Current.LocalSettings;
    }
}
