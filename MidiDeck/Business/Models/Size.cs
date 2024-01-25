namespace MidiDeck.Business.Models;

public class Size
{
    public Size(int rows, int columns)
    {
        Rows = rows;
        Columns = columns;
    }

    public int Rows { get; set; }
    public int Columns { get; set; }
}
