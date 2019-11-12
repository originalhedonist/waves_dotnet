using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace wavegenerator
{
    public static class ConsoleWriter
    {
        private static object lockObj = new object();
        public static void Write(string s, ConsoleColor c)
        {
            lock (lockObj)
            {
                var defaultColor = Console.ForegroundColor;
                Console.ForegroundColor = c;
                Console.Write(s);
                Console.ForegroundColor = defaultColor;
            }
        }
        public static void WriteLine(string s, ConsoleColor c)
        {
            lock (lockObj)
            {
                var defaultColor = Console.ForegroundColor;
                Console.ForegroundColor = c;
                Console.WriteLine(s);
                Console.ForegroundColor = defaultColor;
            }
        }
    }
}
