using MidiController.Services.Models;

namespace MidiController.Services.Interfaces;

public interface ISoundService
{
    Task<Sound> LoadSoundAsync(string path);

    Task UnloadSoundAsync(Sound sound);

   void PlaySound(Sound sound, double volume=0.5);
}
