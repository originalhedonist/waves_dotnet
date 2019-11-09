using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace wavereader
{
    class wavereader
    {
        static void Main(string[] args)
        {
            //read
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Projects/waves/middlec.wav");
            using (var file = File.OpenRead(path))
            {
                var binaryReader = new BinaryReader(file);

                string readString(int length)
                {
                    byte[] b = new byte[length];
                    file.Read(b, 0, length);
                    return Encoding.ASCII.GetString(b);
                }

                Debug.WriteLine(readString(4)); //'RIFF'
                Debug.WriteLine($"File size - 8 = {binaryReader.ReadInt32()}");
                Debug.WriteLine($"WAVE = {readString(4)}"); //'WAVE'
                Debug.WriteLine($"fmt = {readString(4)}"); //'fmt '
                Debug.WriteLine($"Length of format data = {binaryReader.ReadInt32()}"); //18 + numsamples*samplelength
                Debug.WriteLine($"Type of format (1 is PCM): {binaryReader.ReadInt16()}");// channels
                Debug.WriteLine($"Num channels: {binaryReader.ReadInt16()}");//sample rate
                Debug.WriteLine($"Sample rate: {binaryReader.ReadInt32()}");//samplerate*samplelength*numchannels = average bytes per second
                Debug.WriteLine($"Sample rate*bits per channel*samples/8 = {binaryReader.ReadInt32()}");
                Debug.WriteLine($"Bits per sample * channels/8 = {binaryReader.ReadInt16()}");
                Debug.WriteLine($"Bits per sample: {binaryReader.ReadInt16()}");//samplelength*numchannels = block align
                Debug.WriteLine($"data = {readString(4)}");//8*samplelength = bits per sample
                Debug.WriteLine($"data size = {binaryReader.ReadInt32()}");//numsamples*samplelength = extra size
                Debug.WriteLine($"position (should be 44) = {file.Position}");
                for(int n = 0; n < 10; n++)
                {
                    Debug.WriteLine($"a({n}) = {binaryReader.ReadInt16()}");
                }
            }
        }
    }
}
