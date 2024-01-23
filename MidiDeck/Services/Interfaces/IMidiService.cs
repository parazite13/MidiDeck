using Windows.Devices.Enumeration;
using Windows.Devices.Midi;

namespace MidiDeck.Services.Interfaces;

public interface IMidiService
{
    event Windows.Foundation.TypedEventHandler<MidiInPort, MidiMessageReceivedEventArgs> OnMidiInput;

    Task StartMidiInputWatcherAsync(DeviceInformation midiDevice);
}
