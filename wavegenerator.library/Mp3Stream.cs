using Microsoft.Extensions.Logging;
using NAudio.Lame;
using NAudio.Wave;
using System;
using System.IO;
using System.Threading.Tasks;

namespace wavegenerator.library
{
    public class Mp3Stream
    {
        private readonly WaveStream waveStream;
        private readonly IProgressReporter progressReporter;
        private readonly ILogger logger;

        public Mp3Stream(WaveStream waveStream, IProgressReporter progressReporter, ILoggerFactory loggerFactory)
        {
            this.waveStream = waveStream;
            this.progressReporter = progressReporter;
            this.logger = loggerFactory.CreateLogger(typeof(Mp3Stream));
        }

        public async Task Write(string filePath)
        {
            logger.LogInformation("Creating mp3");
            try
            {
                //don't write direct to the file - otherwise it's well slow.
                await using var fileStream = new BufferedStream(File.OpenWrite(filePath));
                await Write(fileStream);

            }
            catch (Exception e)
            {
                logger.LogError(e, "Error creating mp3");
                throw;
            }
        }

        public async Task Write(Stream fileStream)
        {
            string wavIntermediate = Guid.NewGuid() + ".wav";
            {
                await waveStream.Write(wavIntermediate);
                await progressReporter.ReportProgress(0.97, false, "Converting WAV to MP3");
                await using var audioFileReader = new AudioFileReader(wavIntermediate);
                await using var writer = new LameMP3FileWriter(fileStream, audioFileReader.WaveFormat, LAMEPreset.ABR_320);
                await audioFileReader.CopyToAsync(writer);
            }
            await progressReporter.ReportProgress(1, true, "File created successfully");
            File.Delete(wavIntermediate);
        }
    }
}