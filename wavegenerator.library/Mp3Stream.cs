using Microsoft.Extensions.Logging;
using NAudio.Lame;
using NAudio.Wave;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using wavegenerator.models;

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
                CheckAddBinPath();
                //don't write direct to the file - otherwise it's well slow.
                await using var fileStream = new BufferedStream(File.OpenWrite(filePath));
                await Write(fileStream);

            }
            catch (Exception e)
            {
                logger.LogError(e, "Error creating mp3");
                await progressReporter.ReportProgress(1, JobProgressStatus.Failed, "An error occcurred creating the file");
                throw;
            }
        }

        public async Task Write(Stream fileStream)
        {
            string wavIntermediate = Path.Combine("DownloadableFiles", Guid.NewGuid() + ".wav");
            try
            {
                {
                    await waveStream.Write(wavIntermediate);
                    await progressReporter.ReportProgress(0.97, JobProgressStatus.InProgress, "Converting WAV to MP3");
                    await using var audioFileReader = new AudioFileReader(wavIntermediate);
                    await using var writer = new LameMP3FileWriter(fileStream, audioFileReader.WaveFormat, LAMEPreset.ABR_320);
                    await audioFileReader.CopyToAsync(writer);

                } // dispose it before reporting complete
                await progressReporter.ReportProgress(1, JobProgressStatus.Complete, "File created successfully");
            }
            finally
            {
                if (File.Exists(wavIntermediate))
                {
                    try { File.Delete(wavIntermediate); } catch (Exception) { }
                }
            }
        }

        public void CheckAddBinPath() // necessary to load libmp3lame.32.dll
        {
            // find path to 'bin' folder
            var binPath = Path.Combine(new string[] { AppDomain.CurrentDomain.BaseDirectory });
            // get current search path from environment

            var path = Environment.GetEnvironmentVariable("PATH") ?? "";
            logger.LogInformation($"path = {path}");
            logger.LogInformation($"binPath = {binPath}");
            // add 'bin' folder to search path if not already present
            if (!path.Split(Path.PathSeparator).Contains(binPath, StringComparer.CurrentCultureIgnoreCase))
            {
                path = string.Join(Path.PathSeparator.ToString(), new string[] { path, binPath });
                Environment.SetEnvironmentVariable("PATH", path);
            }
        }
    }
}