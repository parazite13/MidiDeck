using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Windows.Media.Core;

namespace MidiDeck.Services.Models
{
    public class Sound : IDisposable
    {
        private MediaSource mediaSource;
        public MediaSource MediaSource => mediaSource;

        private Sound() { }

        public static async Task<Sound> FromPathAsync(string path)
        {
            var sound = new Sound();
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Create("BROWSER")))
            {

            }
            var storageFile = await StorageFile.GetFileFromPathAsync(path);
            sound.mediaSource = MediaSource.CreateFromStorageFile(storageFile);
            return sound;
        }

        public void Dispose()
        {
            mediaSource?.Dispose();
        }
    }
}
