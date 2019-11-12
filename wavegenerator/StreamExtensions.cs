using System.IO;
using System.Threading.Tasks;

namespace wavegenerator
{
    public static class StreamExtensions
    {
        public static async Task WriteAsync(this Stream s, short value)
        {
            var _buffer = new byte[2];
            _buffer[0] = (byte)value;
            _buffer[1] = (byte)(value >> 8);
            await s.WriteAsync(_buffer, 0, 2);
        }
        public static async Task WriteAsync(this Stream s, int value)
        {
            var _buffer = new byte[4];
            _buffer[0] = (byte)value;
            _buffer[1] = (byte)(value >> 8);
            _buffer[2] = (byte)(value >> 16);
            _buffer[3] = (byte)(value >> 24);
            await s.WriteAsync(_buffer, 0, 4);
        }
    }
}
