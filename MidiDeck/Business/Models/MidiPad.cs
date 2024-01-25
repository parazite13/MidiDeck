using MidiDeck.Services.Interfaces;
using Windows.Devices.Midi;
using Windows.Media.Core;

namespace MidiDeck.Business.Models;

public class MidiPad
{
    public string Name { get; set; } = "Test";

    public byte Note { get; set; }

    public double Volume { get; set; } = 50;

    public string Path { get; set; }
}
