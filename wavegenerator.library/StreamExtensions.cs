using System.IO;
using System.Threading.Tasks;

namespace wavegenerator.library
{
    public static class StreamExtensions
    {
        public static async Task WriteAsync(this Stream s, short value)
        {
            var buffer = new byte[2];
            buffer[0] = (byte) value;
            buffer[1] = (byte) (value >> 8);
            await s.WriteAsync(buffer, 0, 2);
        }

        public static async Task WriteAsync(this Stream s, int value)
        {
            var buffer = new byte[4];
            buffer[0] = (byte) value;
            buffer[1] = (byte) (value >> 8);
            buffer[2] = (byte) (value >> 16);
            buffer[3] = (byte) (value >> 24);
            await s.WriteAsync(buffer, 0, 4);
        }

        public static Task WriteAsync(this Stream s, byte[] b)
        {
            return s.WriteAsync(b, 0, b.Length);
        }
    }
}