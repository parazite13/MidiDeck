using MidiController.Services.Interfaces;

namespace MidiController.Services;
public class SettingService : ISettingsService
{
    private readonly ApplicationDataContainer dataContainer;

    public SettingService() 
    {
        dataContainer = ApplicationData.Current.LocalSettings;
    }
}
