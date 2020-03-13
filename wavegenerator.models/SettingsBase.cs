using System;
using System.ComponentModel.DataAnnotations;

namespace wavegenerator.models
{
    public class SettingsBase
    {
        public int Version { get; set; }

        [ValidateNamingConvention]
        public NamingConvention Naming { get; set; }

        [Range(typeof(TimeSpan), "00:00:30", "13:31:35")]
        public TimeSpan TrackLength { get; set; }
    }
}
