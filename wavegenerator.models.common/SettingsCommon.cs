﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace wavegenerator.models
{
    public class SettingsCommon
    {
        public int? Version { get; set; }

        [ValidateNamingConvention]
        public NamingConvention Naming { get; set; }

        [Range(typeof(TimeSpan), "00:00:30", "13:31:35")]
        public TimeSpan TrackLength { get; set; }

        [Description("Whether to use randomization")]
        public bool Randomization { get; set; }

        public virtual short GetNumberOfChannels() => 2;
    }
}
