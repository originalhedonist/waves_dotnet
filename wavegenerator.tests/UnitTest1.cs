using System;
using System.IO;
using Xunit;
using Xunit.Abstractions;

namespace wavegenerator.tests
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper output;

        public UnitTest1(ITestOutputHelper output)
        {
            this.output = output;
        }
        [Fact]
        public void CreateFile1()
        {
            output.WriteLine(Environment.CurrentDirectory);
            Assert.True(File.Exists("Settings1.json"));
        }
    }
}
