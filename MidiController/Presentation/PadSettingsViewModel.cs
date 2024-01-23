using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiController.Presentation;
public partial class PadSettingsViewModel : ObservableObject
{
    private readonly INavigator navigator;

    [ObservableProperty]
    private MidiPad pad;

    public PadSettingsViewModel(
        INavigator navigator)
    {
        this.navigator = navigator;
    }

}
