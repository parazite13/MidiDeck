using Microsoft.UI.Xaml.Media.Imaging;
using Windows.ApplicationModel.DataTransfer;


namespace MidiDeck.Presentation;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
    }

    private async void Button_Drop(object sender, DragEventArgs e)
    {
#if !__ANDROID__
        if (e.DataView.Contains(StandardDataFormats.StorageItems))
        {
            var items = await e.DataView.GetStorageItemsAsync();
            if (items.Count > 0)
            {
                var storageFile = items[0] as StorageFile;
                var bitmapImage = new BitmapImage();
                bitmapImage.SetSource(await storageFile.OpenAsync(FileAccessMode.Read));
            }
        }
#endif
    }

    private void Button_DragEnter(object sender, DragEventArgs e)
    {

    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        if(DataContext is MainViewModel viewModel)
        {
            await viewModel.Init();
        }
    }
}
