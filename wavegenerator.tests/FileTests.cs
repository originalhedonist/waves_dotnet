﻿using System.IO;
using System.Threading.Tasks;
using wavegenerator.library;
using Xunit;
using Xunit.Abstractions;

namespace wavegenerator.tests
{
    public class FileTests
    {
        private readonly ITestOutputHelper testOutputHelper;

        public FileTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task Mp3ToFile()
        {
            if(!Directory.Exists("Output"))
            {
                Directory.CreateDirectory("Output");
            }

            var settings = SettingsLoader.LoadAndValidateSettings("Settings_FrequencyLinked.json");
            var container = DependencyConfig.ConfigureContainer(settings, c =>
            {
                c.AddInstance<IProgressReporter>(new XUnitProgressReporter(testOutputHelper));
            });

            var mp3Stream = container.Resolve<Mp3Stream>();
            await mp3Stream.Write("Output/TestFile.mp3");
        }
    }
}
