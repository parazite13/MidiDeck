using MidiDeck.Services.Interfaces;
using Windows.Devices.Midi;
using Windows.Media.Core;

namespace MidiDeck.Business.Models;

public class MidiPad
{
    public string Name { get; set; }

    public byte Note { get; set; }

    public double Volume { get; set; } = 0.5;

    public string Path => @$"E:\Data\Alexandre\Music\Pads\Robot Rock\{Math.Min(Note-35, 15)}.wav";
}
