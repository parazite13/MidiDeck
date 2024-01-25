using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage.Pickers;

namespace MidiDeck.Presentation;
public partial class PadSettingsViewModel : ObservableObject
{
    private readonly INavigator navigator;

    public ICommand BrowseFileCommand { get; }

    [ObservableProperty]
    private MidiPad pad;

    public PadSettingsViewModel(
        MidiPad pad,
        INavigator navigator)
    {
        this.pad = pad;
        this.navigator = navigator;

        BrowseFileCommand = new AsyncRelayCommand(BrowseFile);
    }

    private async Task BrowseFile()
    {
        var fileOpenPicker = new FileOpenPicker();
        fileOpenPicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
        fileOpenPicker.FileTypeFilter.Add(".wav");

        // For Uno.WinUI-based apps
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
        WinRT.Interop.InitializeWithWindow.Initialize(fileOpenPicker, hwnd);

        StorageFile pickedFile = await fileOpenPicker.PickSingleFileAsync();
        if (pickedFile != null)
        {
            Pad.Path = pickedFile.Path;
            OnPropertyChanged(nameof(Pad));
        }
    }

}
