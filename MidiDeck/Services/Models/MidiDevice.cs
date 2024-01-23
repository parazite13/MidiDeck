using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiDeck.Services.Models;

public class MidiDevice : IEquatable<MidiDevice?>
{
    public string Id { get; set; }
    public string Name { get; set; }
    public bool IsEnabled { get; set; }

    public override bool Equals(object? obj)
    {
        return Equals(obj as MidiDevice);
    }

    public bool Equals(MidiDevice? other)
    {
        return other is not null &&
               Id == other.Id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id);
    }
}
