using Microsoft.Xna.Framework.Audio;
using Nez;
using Nez.Audio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Triton.Audio.Decoders;

namespace Fgj22.App.Components
{
    public class SoundEffectPlayer : Component
    {
        string Filename;
        private SoundEffectInstance Sound;

        public SoundEffectPlayer(string filename)
        {
            Filename = filename;
        }

        public override void OnAddedToEntity()
        {
            var path = Path.Combine(Entity.Scene.Content.RootDirectory, "soundEffects", $"{Filename}.ogg");
            var effect = new SoundEffect(OggDecoder.ReadContentFile(path), 44100, AudioChannels.Stereo);

            Sound = effect.CreateInstance();
            Sound.Play();
        }

        public override void OnRemovedFromEntity()
        {
            Sound.Stop();
        }
    }
}
