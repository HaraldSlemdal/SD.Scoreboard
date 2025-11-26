using NAudio.Wave;

namespace SD.Scoreboard
{
    // Caches an entire audio file into memory as 32-bit float samples
    public sealed class CachedSound
    {
        public float[] AudioData { get; }
        public WaveFormat WaveFormat { get; }

        public CachedSound(string fileName)
        {
            using (var reader = new AudioFileReader(fileName)) // always float 44.1k stereo by default
            {
                WaveFormat = reader.WaveFormat;
                var wholeFile = new System.Collections.Generic.List<float>((int)(reader.Length / 4));
                var buffer = new float[reader.WaveFormat.SampleRate * reader.WaveFormat.Channels];
                int read;
                while ((read = reader.Read(buffer, 0, buffer.Length)) > 0)
                {
                    if (read < buffer.Length)
                    {
                        Array.Resize(ref buffer, read);
                    }

                    wholeFile.AddRange(buffer.Take(read));
                    buffer = new float[reader.WaveFormat.SampleRate * reader.WaveFormat.Channels];
                }

                AudioData = wholeFile.ToArray();
            }
        }
    }

    // Plays back from a CachedSound without reopening/decoding the file each time
    public sealed class CachedSoundSampleProvider : ISampleProvider
    {
        private readonly CachedSound _cached;
        private long _position;

        public CachedSoundSampleProvider(CachedSound cached)
        {
            _cached = cached;
        }

        public WaveFormat WaveFormat => _cached.WaveFormat;

        public int Read(float[] buffer, int offset, int count)
        {
            var availableSamples = _cached.AudioData.Length - _position;
            var samplesToCopy = (int)Math.Min(availableSamples, count);
            Array.Copy(_cached.AudioData, _position, buffer, offset, samplesToCopy);
            _position += samplesToCopy;
            return samplesToCopy;
        }
    }
}