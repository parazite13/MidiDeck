using MidiController.Services.Interfaces;
using MidiController.Services.Models;
using Windows.Media.Core;
using Windows.Media.Playback;

namespace MidiController.Services;

public class SoundService : ISoundService
{
    private readonly Dictionary<Sound, MediaPlayer> sounds = new();

    public SoundService()
    {
    }

    public async Task<Sound> LoadSoundAsync(string path)
    {
        var sound = await Sound.FromPathAsync(path);
        var mediaPlayer = new MediaPlayer
        {
            Source = sound.MediaSource,
            RealTimePlayback = true,
        };
        sounds.Add(sound, mediaPlayer);
        return sound;
    }

    public Task UnloadSoundAsync(Sound sound)
    {
        if(sounds.ContainsKey(sound))
        {
            sounds[sound].Dispose();
            sound.Dispose();
        }
        sounds.Remove(sound);
        return Task.CompletedTask;
    }

    public void PlaySound(Sound sound, double volume = 0.5)
    {
        if(sounds.ContainsKey(sound))
        {
            sounds[sound].Volume = volume;
            if (sounds[sound].CurrentState == MediaPlayerState.Playing)
            {
                //sounds[sound].Stop();
            }
            sounds[sound].Play();
        }
    }
}
