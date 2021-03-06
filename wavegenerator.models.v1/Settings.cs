﻿using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace wavegenerator.models
{
    public class Settings : SettingsCommon, IWaveFileMetadata
    {
        public double TrackLengthSeconds() => TrackLength.TotalSeconds;

        [Range(1, 2)]
        public short NumberOfChannels { get; set; }
        public override short GetNumberOfChannels() => NumberOfChannels;

        [Description("Whether the right channel's carrier signal will be phase shifted from the left's")]
        public bool PhaseShiftCarrier { get; set; }

        [Description("Whether the pulsing of the right channel will be phase-shifted from the left")]
        public bool PhaseShiftPulses { get; set; }

        [ValidateObject]
        [ValidateChannelCount]
        public ChannelSettingsModel[] ChannelSettings { get; set; }

        public int NumSections(int channel, Settings settings) => (int)(TrackLength.TotalSeconds / ChannelSettings.ForChannel(settings, channel).Sections.TotalLength.TotalSeconds);
    }

    public static class SettingsExtensions
    {
        public static T ForChannel<T>(this T[] items, Settings settings, int channel) => settings.ChannelSettings.Length > 1 ? items[channel] : items.Single();
    }
}
