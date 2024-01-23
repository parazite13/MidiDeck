
namespace MidiDeck.Presentation;

public sealed partial class MidiSettingsPage : Page
{
    public MidiSettingsPage()
    {
        this.InitializeComponent();
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        if(DataContext is MidiSettingsViewModel viewModel)
        {
            await viewModel.LoadMidiDevices();
        }
    }
}

