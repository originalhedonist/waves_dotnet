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
            //C6CE387BCE819FA67AC23EEE999DA4A22A2248D02C6A33B98448FEAE469A0FD3
            //5929D9FF329610B930D5316F952F9FFEBED1530376DAA9A3843C0B6DF0DDC30E
        }
    }
}
