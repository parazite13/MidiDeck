using MidiDeck.Services.Interfaces;
using Windows.Devices.Midi;
using Windows.Devices.Enumeration;
using MidiDeck.Services.Models;

namespace MidiDeck.Services;

public class MidiService : IMidiService
{
    public event Windows.Foundation.TypedEventHandler<MidiInPort, MidiMessageReceivedEventArgs> OnMidiInput;

    private readonly Dictionary<MidiDevice, MidiInPort> midiInPorts = new();
    private readonly Dictionary<MidiDevice, MidiOutPort> midiOutPorts = new();

    public IReadOnlyCollection<MidiDevice> MidiInWatchedDevices => midiInPorts.Keys;
    public IReadOnlyCollection<MidiDevice> MidiOutWatchedDevices => midiOutPorts.Keys;

    public MidiService()
    {
#if __WASM__
        WinRTFeatureConfiguration.Midi.RequestSystemExclusiveAccess = true;
#endif
    }

    public async Task<IReadOnlyCollection<MidiDevice>> GetAvailableInputDevices()
    {
        var midiInputQueryString = MidiInPort.GetDeviceSelector();
        var devices = await DeviceInformation.FindAllAsync(midiInputQueryString);
        return devices.Select(d => new MidiDevice
        {
            Id = d.Id,
            Name = d.Name,
            IsEnabled = MidiInWatchedDevices.Any(watched => watched.Id == d.Id),
        }).ToList().AsReadOnly();
    }

    public async Task<IReadOnlyCollection<MidiDevice>> GetAvailableOutputDevices()
    {
        var midiOutputQueryString = MidiOutPort.GetDeviceSelector();
        var devices = await DeviceInformation.FindAllAsync(midiOutputQueryString);
        return devices.Select(d => new MidiDevice
        {
            Id = d.Id,
            Name = d.Name,
            IsEnabled = MidiOutWatchedDevices.Any(watched => watched.Id == d.Id),
        }).ToList().AsReadOnly();
    }

    public async Task StartMidiInputWatcherAsync(MidiDevice midiDevice)
    {
        if(!midiInPorts.ContainsKey(midiDevice))
        {
            var midiInPort = await MidiInPort.FromIdAsync(midiDevice.Id);
            midiInPort.MessageReceived += MidiIn_MessageReceived;
            midiInPorts.Add(midiDevice, midiInPort);
        }
    }

    public Task StopMidiInputWatcherAsync(MidiDevice midiDevice)
    {
        if (midiInPorts.ContainsKey(midiDevice))
        {
            midiInPorts[midiDevice].MessageReceived -= MidiIn_MessageReceived;
            midiInPorts[midiDevice].Dispose();
            midiInPorts.Remove(midiDevice);
        }
        return Task.CompletedTask;
    }

    private void MidiIn_MessageReceived(MidiInPort sender, MidiMessageReceivedEventArgs args)
    {
        OnMidiInput?.Invoke(sender, args);
    }
}
