
namespace MidiDeck.Presentation;

public partial class LayoutSettingsViewModel : ObservableObject
{
    [ObservableProperty]
    private Size size;

    public ICommand ApplyCommand { get; }
    public ICommand CancelCommand { get; }

    private readonly IStringLocalizer localizer;
    private readonly INavigator navigator;

    private Size initialSize;

    public LayoutSettingsViewModel(
        Size size,
        IStringLocalizer localizer,
        INavigator navigator)
    {
        this.navigator = navigator;
        this.localizer = localizer;    
        
        ApplyCommand = new AsyncRelayCommand(Apply);
        CancelCommand = new AsyncRelayCommand(Cancel);

        initialSize = new Size(size.Rows, size.Columns);
        Size = size;
    }


    private async Task Apply()
    {
        await navigator.NavigateBackAsync(this);
    }

    private async Task Cancel()
    {
        Size.Rows = initialSize.Rows;
        Size.Columns = initialSize.Columns;
        await navigator.NavigateBackAsync(this);
    }
}
