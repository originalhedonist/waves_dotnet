using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace wavegenerator
{
    public abstract class WaveStream : IAmplitude
    {
        protected const short bytesPerSample = 2;
        public double LengthSeconds { get; }
        public short Channels { get; }
        protected readonly int overallDataSize;
        protected readonly int overallFileSize;
        protected readonly int N;

        public WaveStream()
        {
            this.LengthSeconds = Settings.Instance.TrackLength.TotalSeconds;
            this.Channels = Settings.Instance.NumberOfChannels;
            this.N = (int)(this.LengthSeconds * Settings.SamplingFrequency);
            this.overallDataSize = N * this.Channels * bytesPerSample;
            this.overallFileSize = this.overallDataSize + 44;
            if (this.Channels < 1 || this.Channels > 2)
            {
                throw new InvalidOperationException("Channels must be either 1 or 2.");
            }
        }

        public async Task Write(string filePath)
        {
            //don't write direct to the file - otherwise it's well slow.
            using (var fileStream = new BufferedStream(File.OpenWrite(filePath)))
            {
                await Write(fileStream);
            }
        }

        public async Task Write(Stream stream)
        {
            await stream.WriteAsync(Encoding.ASCII.GetBytes("RIFF"));
            await stream.WriteAsync(overallFileSize - 8);
            await stream.WriteAsync(Encoding.ASCII.GetBytes("WAVE"));
            await stream.WriteAsync(Encoding.ASCII.GetBytes("fmt "));
            await stream.WriteAsync((int)16); //length of format data
            await stream.WriteAsync((short)1); //type of format (1 = PCM)
            await stream.WriteAsync(Channels);
            await stream.WriteAsync(Settings.SamplingFrequency);
            await stream.WriteAsync((int)(Settings.SamplingFrequency * bytesPerSample * Channels));
            await stream.WriteAsync((short)(bytesPerSample * Channels));
            await stream.WriteAsync((short)(bytesPerSample * 8)); // bits per sample
            await stream.WriteAsync(Encoding.ASCII.GetBytes("data"));
            await stream.WriteAsync(overallDataSize);

            for (int n = 0; n < N; n++)
            {
                double t = LengthSeconds * ((double)n) / N;

                for (int c = 0; c < Channels; c++)
                {
                    double A = await Amplitude(t, n, c);
                    if (A < -1 || A > 1)
                    {
                        throw new InvalidOperationException($"Amplitude must be -1 to 1. Amplitude for n = {n}, c = {c} was {A}.");
                    }

                    short a = (short)(((A + 1) * (65535f / 2)) - 32768);

                    await stream.WriteAsync(a);
                }
            }
        }

        public abstract Task<double> Amplitude(double t, int n, int channel);
    }
}
