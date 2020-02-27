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
            //FrequencyLinked C6CE387BCE819FA67AC23EEE999DA4A22A2248D02C6A33B98448FEAE469A0FD3
            //WetnessLinked 5929D9FF329610B930D5316F952F9FFEBED1530376DAA9A3843C0B6DF0DDC30E
            //FrequencyIndependent 17FA901DBC00BEE55DCEE6F5FA38F8CCB92BD0F47D458F3FC36533F3069CD609
            //WetnessIndependnet 1E120A7FB6271590ABAD2F87822BF408B6D7D6E50CA7A63E358D30D5431713E3
        }
    }
}
