﻿using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace wavegenerator.library
{
    public class WaveStream : IAmplitude
    {
        private const short BytesPerSample = 2;
        public double LengthSeconds { get; }
        public short Channels { get; }
        private readonly int overallDataSize;
        private readonly int overallFileSize;
        private readonly int N;
        private readonly ChannelSplitter channelSplitter;

        public WaveStream(Settings settings, ChannelSplitter channelSplitter)
        {
            LengthSeconds = settings.TrackLength.TotalSeconds;
            Channels = settings.NumberOfChannels;
            N = (int) (LengthSeconds * Settings.SamplingFrequency);
            overallDataSize = N * Channels * BytesPerSample;
            overallFileSize = overallDataSize + 44;
            if (Channels < 1 || Channels > 2) throw new InvalidOperationException("Channels must be either 1 or 2.");

            this.channelSplitter = channelSplitter;
        }

        public Task<double> Amplitude(double t, int n, int channel)
        {
            return channelSplitter.Amplitude(t, n, channel);
        }


        public async Task Write(string filePath)
        {
            //don't write direct to the file - otherwise it's well slow.
            await using var fileStream = new BufferedStream(File.OpenWrite(filePath));
            await Write(fileStream);
        }

        public async Task Write(Stream stream)
        {
            await stream.WriteAsync(Encoding.ASCII.GetBytes("RIFF"));
            await stream.WriteAsync(overallFileSize - 8);
            await stream.WriteAsync(Encoding.ASCII.GetBytes("WAVE"));
            await stream.WriteAsync(Encoding.ASCII.GetBytes("fmt "));
            await stream.WriteAsync((int) 16); //length of format data
            await stream.WriteAsync((short) 1); //type of format (1 = PCM)
            await stream.WriteAsync(Channels);
            await stream.WriteAsync(Settings.SamplingFrequency);
            await stream.WriteAsync((int) (Settings.SamplingFrequency * BytesPerSample * Channels));
            await stream.WriteAsync((short) (BytesPerSample * Channels));
            await stream.WriteAsync((short) (BytesPerSample * 8)); // bits per sample
            await stream.WriteAsync(Encoding.ASCII.GetBytes("data"));
            await stream.WriteAsync(overallDataSize);

            for (var n = 0; n < N; n++)
            {
                var t = LengthSeconds * (double) n / N;

                for (var c = 0; c < Channels; c++)
                {
                    var A = await Amplitude(t, n, c);
                    if (A < -1 || A > 1)
                        throw new InvalidOperationException(
                            $"Amplitude must be -1 to 1. Amplitude for n = {n}, c = {c} was {A}.");

                    var a = (short) ((A + 1) * (65535f / 2) - 32768);

                    await stream.WriteAsync(a);
                }
            }
        }
    }
}