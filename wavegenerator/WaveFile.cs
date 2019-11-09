using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace wavegenerator
{
    public abstract class WaveFile
    {
        protected const int samplingFrequency = 44100;
        protected const short bytesPerSample = 2;
        protected readonly int lengthSeconds;
        protected readonly short channels;
        protected readonly int overallDataSize;
        protected readonly int overallFileSize;
        protected readonly int N;

        public WaveFile(int lengthSeconds, short channels)
        {
            this.lengthSeconds = lengthSeconds;
            this.channels = channels;
            this.N = lengthSeconds * samplingFrequency;
            this.overallDataSize = N * channels * bytesPerSample;
            this.overallFileSize = this.overallDataSize + 44;
            if(channels < 1 || channels > 2)
            {
                throw new InvalidOperationException("Channels must be either 1 or 2.");
            }
        }

        public void Write(string filePath)
        {
            using (var fileStream = File.OpenWrite(filePath))
            {
                using (var binaryWriter = new BinaryWriter(fileStream))
                {
                    fileStream.Write(Encoding.ASCII.GetBytes("RIFF"));
                    binaryWriter.Write(overallFileSize - 8);
                    fileStream.Write(Encoding.ASCII.GetBytes("WAVE"));
                    fileStream.Write(Encoding.ASCII.GetBytes("fmt "));
                    binaryWriter.Write((int)16); //length of format data
                    binaryWriter.Write((short)1); //type of format (1 = PCM)
                    binaryWriter.Write(channels);
                    binaryWriter.Write(samplingFrequency);
                    binaryWriter.Write((int)(samplingFrequency * bytesPerSample * channels));
                    binaryWriter.Write((short)(bytesPerSample * channels));
                    binaryWriter.Write((short)(bytesPerSample * 8)); // bits per sample
                    fileStream.Write(Encoding.ASCII.GetBytes("data"));
                    binaryWriter.Write(overallDataSize);

                    for(int n = 0; n < N; n++)
                    {
                        double t = lengthSeconds * ((double)n) / N;
                        for (int c = 0; c < channels; c++)
                        {
                            double A = Amplitude(t, n, c);
                            if (A < -1 || A > 1)
                            {
                                throw new InvalidOperationException($"Amplitude must be -1 to 1. Amplitude for n = {n}, c = {c} was {A}.");
                            }

                            short a = (short)(((A + 1) * (65535f / 2)) - 32768);
                            binaryWriter.Write(a);
                        }
                    }
                }
            }
        }

        protected abstract double Amplitude(double t, int n, int channel);
    }
}
