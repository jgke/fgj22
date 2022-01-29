using Nez.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Nez;
using Fgj22.App.Components;
using Nez.Tiled;
using Nez.Textures;
using Fgj22.App.Systems;
using Fgj22.App.Utility;
using Nez.UI;
using System;
using System.IO;
using Triton.Audio.Decoders;

namespace Fgj22.App
{
    class MusicPlayer : Component
    {
        SoundEffectInstance music;
        string Filename;

        public MusicPlayer(string filename)
        {
            Filename = filename;
        }

        public override void OnAddedToEntity()
        {
            var path = Path.Combine(Entity.Scene.Content.RootDirectory, Filename);
            SoundEffect mySoundEffect = new SoundEffect(OggDecoder.ReadContentFile(path), 44100, AudioChannels.Stereo);
            music = mySoundEffect.CreateInstance();
            music.IsLooped = true;
            music.Play();
        }

        public override void OnRemovedFromEntity()
        {
            music.Stop();
        }
    }
}