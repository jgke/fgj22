// https://github.com/johang88/triton/blob/master/Triton.Audio/Decoders/OggDecoder.cs MIT license

using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Triton.Audio.Decoders
{
    class OggDecoder
    {
        public int Frequency { get; private set; }
        public bool IsStreamingPrefered { get; private set; }
        private NVorbis.VorbisReader Reader;

        public OggDecoder(FileStream stream)
        {
            Reader = new NVorbis.VorbisReader(stream, true);

            Frequency = Reader.SampleRate;
            IsStreamingPrefered = Reader.TotalTime.TotalSeconds > 10.0f;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                Reader.Dispose();
            }
        }

        public int ReadSamples(float[] buffer, int offset, int count)
        {
            return Reader.ReadSamples(buffer, offset, count);
        }

        public long TotalSamples { get { return Reader.TotalSamples; } }

        public static byte[] ReadContentFile(string path)
        {
            FileStream stream = new FileStream(path, FileMode.Open);
            NVorbis.VorbisReader vorbis = new NVorbis.VorbisReader(stream, true);
            float[] buffer = new float[1024];
            List<byte> result = new List<byte>();
            int count;
            while ((count = vorbis.ReadSamples(buffer, 0, buffer.Length)) > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    short temp = (short)(32767f * buffer[i]);
                    if (temp > 32767)
                    {
                        result.Add(0xFF);
                        result.Add(0x7F);
                    }
                    else if (temp < -32768)
                    {
                        result.Add(0x80);
                        result.Add(0x00);
                    }
                    result.Add((byte)temp);
                    result.Add((byte)(temp >> 8));
                }
            }
            //this.Channels = (ushort)vorbis.Channels;
            //this.SamplesPerSecond = (uint)vorbis.SampleRate;
            //this.Data = result.ToArray();
            return result.ToArray();
        }
    }
}