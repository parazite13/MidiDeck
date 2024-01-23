using System.Collections.ObjectModel;
using MidiDeck.Services.Interfaces;
using MidiDeck.Services.Models;

namespace MidiDeck.Presentation;

public partial class MidiSettingsViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<MidiDevice> midiInputs = new();

    [ObservableProperty]
    private ObservableCollection<MidiDevice> midiOutputs = new();

    private readonly IMidiService midiService;
    private readonly ISettingsService settingsService;

    public ICommand MidiInputCheckCommand { get; }

    public MidiSettingsViewModel(
        IMidiService midiService,
        ISettingsService settingsService)
    {
        this.midiService = midiService;
        this.settingsService = settingsService;

        MidiInputCheckCommand = new AsyncRelayCommand<object>(MidiInputCheck);
    }

    private async Task MidiInputCheck(object? dataContext)
    {
        settingsService.Set("MidiInputs", MidiInputs.Where(m => m.IsEnabled).Select(m => m.Id).ToArray());
    }

    public async Task LoadMidiDevices()
    {
        MidiInputs.Clear();
        MidiInputs.AddRange(await midiService.GetAvailableInputDevices());
        var midiInputSettings = settingsService.Get<string[]>("MidiInputs");
        if(midiInputSettings is not null)
        {
            foreach (var midiInputSetting in midiInputSettings) 
            {
                var device = MidiInputs.FirstOrDefault(m => m.Id == midiInputSetting);
                if(device is not null)
                {
                    device.IsEnabled = true;
                }
            }
        }

        MidiOutputs.Clear();
        MidiOutputs.AddRange(await midiService.GetAvailableOutputDevices());
    }
}
