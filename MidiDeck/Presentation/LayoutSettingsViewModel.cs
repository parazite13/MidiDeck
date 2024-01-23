
namespace MidiDeck.Presentation;

public partial class LayoutSettingsViewModel : ObservableObject
{
    [ObservableProperty]
    private Size size;

    public ICommand ApplyCommand { get; }
    public ICommand CancelCommand { get; }

    private readonly IStringLocalizer localizer;
    private readonly INavigator navigator;

    public LayoutSettingsViewModel(
        IStringLocalizer localizer,
        INavigator navigator)
    {
        this.navigator = navigator;
        this.localizer = localizer;    

        ApplyCommand = new AsyncRelayCommand(Apply);
        CancelCommand = new AsyncRelayCommand(Cancel);

        Size = new Size(4, 4);
    }


    private async Task Apply()
    {
        await navigator.NavigateBackWithResultAsync(this, qualifier: Qualifiers.NavigateBack, Option.Some(Size));
    }

    private async Task Cancel()
    {
        await navigator.NavigateBackWithResultAsync(this, qualifier: Qualifiers.NavigateBack);
    }
}
