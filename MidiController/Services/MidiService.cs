using MidiController.Services.Interfaces;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Devices.Midi;
using Windows.Devices.Enumeration;

namespace MidiController.Services;

public class MidiService : IMidiService
{
    public event Windows.Foundation.TypedEventHandler<MidiInPort, MidiMessageReceivedEventArgs> OnMidiInput;

    public MidiService()
    {
#if __WASM__
        WinRTFeatureConfiguration.Midi.RequestSystemExclusiveAccess = true;
#endif
    }

    public async Task StartMidiInputWatcherAsync(DeviceInformation midiDevice)
    {
        var midiIn = await MidiInPort.FromIdAsync(midiDevice.Id);
        midiIn.MessageReceived += MidiIn_MessageReceived;
    }

    private void MidiIn_MessageReceived(MidiInPort sender, MidiMessageReceivedEventArgs args)
    {
        OnMidiInput?.Invoke(sender, args);
    }
}
