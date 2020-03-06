using NAudio.Lame;
using NAudio.Wave;
using System.IO;
using System.Threading.Tasks;

namespace wavegenerator.library
{
    public class Mp3Stream
    {
        private readonly WaveStream waveStream;
        private readonly IProgressReporter progressReporter;

        public Mp3Stream(WaveStream waveStream, IProgressReporter progressReporter)
        {
            this.waveStream = waveStream;
            this.progressReporter = progressReporter;
        }

        public async Task Write(string filePath)
        {
            string wavIntermediate = filePath + ".wav";
            {
                await waveStream.Write(wavIntermediate);
                await progressReporter.ReportProgress(0.97, false, "Converting WAV to MP3");
                await using var audioFileReader = new AudioFileReader(wavIntermediate);
                await using var writer = new LameMP3FileWriter(filePath, audioFileReader.WaveFormat, LAMEPreset.ABR_320);
                await audioFileReader.CopyToAsync(writer);
            }
            await progressReporter.ReportProgress(1, true, "File created successfully");
            File.Delete(wavIntermediate);
        }
    }
}