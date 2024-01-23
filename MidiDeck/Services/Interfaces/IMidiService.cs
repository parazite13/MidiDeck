using MidiDeck.Services.Models;
using Windows.Devices.Enumeration;
using Windows.Devices.Midi;

namespace MidiDeck.Services.Interfaces;

public interface IMidiService
{
    event Windows.Foundation.TypedEventHandler<MidiInPort, MidiMessageReceivedEventArgs> OnMidiInput;

    IReadOnlyCollection<MidiDevice> MidiInWatchedDevices { get; }
    IReadOnlyCollection<MidiDevice> MidiOutWatchedDevices { get; }

    Task<IReadOnlyCollection<MidiDevice>> GetAvailableInputDevices();
    Task<IReadOnlyCollection<MidiDevice>> GetAvailableOutputDevices();

    Task StartMidiInputWatcherAsync(MidiDevice midiDevice);
    Task StopMidiInputWatcherAsync(MidiDevice midiDevice);
}
