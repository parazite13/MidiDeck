using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.UI.Dispatching;
using MidiDeck.Services.Interfaces;
using MidiDeck.Services.Models;
using Uno;
using Uno.Extensions.Navigation;
using Windows.Devices.Enumeration;
using Windows.Devices.Midi;
using Windows.Media.Core;
using Windows.Storage.Pickers;

namespace MidiDeck.Presentation;

public partial class MainViewModel : ObservableObject
{
    public string Title => localizer["ApplicationName"];

    [ObservableProperty]
    private MidiLayout? currentLayout;

    private bool isLoading;
    public bool IsLoading
    {
        get => isLoading;
        set
        {
            SetProperty(ref isLoading, value);
            OnPropertyChanged(nameof(PadVisibility));
        }
    }

    public Visibility PadVisibility => IsLoading ? Visibility.Collapsed : Visibility.Visible;

    private readonly INavigator navigator;
    private readonly IStringLocalizer localizer;
    private readonly ISettingsService settings;
    private readonly IMidiService midiService;
    private readonly ISoundService soundService;

    public ICommand NewLayoutCommand { get; }
    public ICommand OpenLayoutCommand { get; }
    public ICommand SaveLayoutCommand { get; }
    public ICommand GoToLayoutSettingsCommand { get; }
    public ICommand GoToMidiSettingsCommand { get; }
    public ICommand PadClickCommand { get; }
    public ICommand PadEditCommand { get; }

    private readonly Dictionary<MidiPad, Sound> sounds = new();

    public MainViewModel(
        IStringLocalizer localizer,
        INavigator navigator,
        ISettingsService settings,
        IMidiService midiService,
        ISoundService soundService)
    {
        this.navigator = navigator;
        this.localizer = localizer;
        this.settings = settings;
        this.midiService = midiService;
        this.soundService = soundService;

        NewLayoutCommand = new AsyncRelayCommand(NewLayout);
        OpenLayoutCommand = new AsyncRelayCommand(OpenLayout);
        SaveLayoutCommand = new AsyncRelayCommand(SaveLayout);
        GoToLayoutSettingsCommand = new AsyncRelayCommand(GoToLayoutSettings);
        GoToMidiSettingsCommand = new AsyncRelayCommand(GoToMidiSettings);
        PadClickCommand = new RelayCommand<object>(PadClick);
        PadEditCommand = new AsyncRelayCommand<object>(PadEdit);

        midiService.OnMidiInput += OnMidiInput;
    }

    ~MainViewModel() 
    {
        midiService.OnMidiInput -= OnMidiInput;
    }

    public Task Init()
    {
        return Task.WhenAll(new List<Task>()
        {
            InitMidiDevices(),
            InitSounds()
        });
    }

    private async Task InitMidiDevices()
    {
        foreach(var device in midiService.MidiInWatchedDevices)
        {
            await midiService.StopMidiInputWatcherAsync(device);
        }

        var devices = await midiService.GetAvailableInputDevices();
        var midiDevices = settings.Get<string[]>("MidiInputs");
        if(midiDevices is not null)
        {
            foreach(var midiDevice in midiDevices)
            {
                var device = devices.FirstOrDefault(d => d.Id == midiDevice);
                if (device is not null)
                {
                    await midiService.StartMidiInputWatcherAsync(device);
                }
            }
        }        
    }

    private async Task InitSounds()
    {
        if(!IsLoading)
        {
            try
            {
                IsLoading = true;

                await Task.Run(async () =>
                {
                    foreach (var sound in sounds.Values)
                    {
                       await soundService.UnloadSoundAsync(sound);
                    }
                    sounds.Clear();

                    var loadingTasks = new Dictionary<MidiPad, Task<Sound>>();
                    if(CurrentLayout is not null)
                    {
                        foreach (var midiPad in CurrentLayout.PadsList)
                        {
                            loadingTasks.Add(midiPad, soundService.LoadSoundAsync(midiPad.Path));
                        }
                        foreach (var task in loadingTasks)
                        {
                            sounds.Add(task.Key, await task.Value);
                        }
                    }
                });
            }
            catch
            {
                // TODO
               // throw;
            }
            finally
            {
                IsLoading = false;
            }
        }
    }

    private async Task NewLayout()
    {
        CurrentLayout = new MidiLayout(4, 4);
        await Init();
    }

    private async Task OpenLayout()
    {
        var fileOpenPicker = new FileOpenPicker();
        fileOpenPicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
        fileOpenPicker.FileTypeFilter.Add(".json");

        // For Uno.WinUI-based apps
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
        WinRT.Interop.InitializeWithWindow.Initialize(fileOpenPicker, hwnd);

        StorageFile pickedFile = await fileOpenPicker.PickSingleFileAsync();
        if (pickedFile != null)
        {
            try
            {
                var content = await FileIO.ReadTextAsync(pickedFile);
                CurrentLayout = JsonSerializer.Deserialize<MidiLayout>(content);
                await Init();
            }
            catch(Exception e)
            {
                // TODO
                throw;
            }
        }
    }

    private async Task SaveLayout()
    {
        if(CurrentLayout is not null)
        {
            var fileSavePicker = new FileSavePicker();
            fileSavePicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            fileSavePicker.SuggestedFileName = "layout.json";
            fileSavePicker.FileTypeChoices.Add("Layout", new List<string>() { ".json" });

            // For Uno.WinUI-based apps
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
            WinRT.Interop.InitializeWithWindow.Initialize(fileSavePicker, hwnd);

            StorageFile saveFile = await fileSavePicker.PickSaveFileAsync();
            if (saveFile != null)
            {
                try
                {
                    CachedFileManager.DeferUpdates(saveFile);

                    var content = JsonSerializer.Serialize(CurrentLayout);
                    await FileIO.WriteTextAsync(saveFile, content);

                    await CachedFileManager.CompleteUpdatesAsync(saveFile);
                }
                catch (Exception e)
                {
                    // TODO
                    throw;
                }
            }
        }
    }

    private async Task GoToLayoutSettings()
    {
        await navigator.NavigateViewModelAsync<LayoutSettingsViewModel>(this, data: CurrentLayout.Size);
    }

    private async Task GoToMidiSettings()
    {
        await navigator.NavigateViewModelAsync<MidiSettingsViewModel>(this);
    }

    private void OnMidiInput(MidiInPort sender, MidiMessageReceivedEventArgs args)
    {
        if (args.Message is MidiNoteOnMessage midiOn)
        {
            if(CurrentLayout is not null)
            {
                var tappedPad = CurrentLayout.PadsList.FirstOrDefault(p => p.Note == midiOn.Note);
                if (tappedPad is not null)
                {
                    PadClick(tappedPad);
                }
            }
        }
    }

    private void PadClick(object? dataContext)
    {
        if(dataContext is MidiPad pad)
        {
            soundService.PlaySound(sounds[pad], pad.Volume / 100d);
        }
    }

    private async Task PadEdit(object? dataContext)
    {
        if (dataContext is MidiPad pad)
        {
            await navigator.NavigateViewModelAsync<PadSettingsViewModel>(this, data: pad);
        }
    }
}
