using System.Text.Json.Serialization;

namespace MidiController.Business.Models;

public class MidiLayout
{
    public Size Size { get; set; }
    
    public MidiPad[][] Pads { get; set; }

    [JsonIgnore]
    public MidiPad[] PadsList => Pads.SelectMany(array => array).ToArray();

    [JsonConstructor]
    public MidiLayout(Size size)
    {
        Size = size;
        Pads = new MidiPad[size.Rows][];
        for (var i = 0; i < size.Rows; i++)
        {
            Pads[i] = new MidiPad[size.Columns];
            for (var j = 0; j < size.Columns; j++)
            {
                Pads[i][j] = new MidiPad()
                {
                    Note = (byte)(i * Size.Columns + j + 36)
                };
            }
        }
    }

    public MidiLayout(int rows, int columns) : this(new Size(rows, columns))
    {

    }
}
