using System;
using System.Linq;
using AutoMapper;
using wavegenerator;
using wavegenerator.models;
using waveweb.ServiceModel;

namespace waveweb.ServiceInterface
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<FeatureProbability, FeatureProbabilityModel>()
                .ForMember(d => d.Frequency, c => c.MapFrom(s => s.FrequencyWeighting / s.Total()))
                .ForMember(d => d.Wetness, c => c.MapFrom(s => s.WetnessWeighting / s.Total()))
                ;
            CreateMap<FeatureProbabilityModel, FeatureProbability>()
                .ForMember(d => d.FrequencyWeighting, c => c.MapFrom(s => s.Frequency))
                .ForMember(d => d.WetnessWeighting, c => c.MapFrom(s => s.Wetness))
                .ForMember(d => d.NothingWeighting, c => c.MapFrom(s => 1 - (s.Frequency + s.Wetness)))
                ;

            CreateMap<CarrierFrequency, CarrierFrequencyModel>();
            CreateMap<CarrierFrequencyModel, CarrierFrequency>();
            CreateMap<PulseFrequency, PulseFrequencyModel>();
            CreateMap<PulseFrequencyModel, PulseFrequency>();
            CreateMap<Variance, VarianceModel>();
            CreateMap<VarianceModel, Variance>();
            CreateMap<Wetness, WetnessModel>()
                .ForMember(d => d.Minimum, c => c.MapFrom(s => s.AmountRange[0]))
                .ForMember(d => d.Maximum, c => c.MapFrom(s => s.AmountRange[1]))
                ;
            CreateMap<WetnessModel, Wetness>()
                .ForMember(d => d.AmountRange, c => c.MapFrom(s => new[] { s.Minimum, s.Maximum }))
                ;

            CreateMap<Breaks, BreaksModel>()
                .ForMember(d => d.MinTimeSinceStartOfTrack, c => c.MapFrom(s => TimeSpan.FromMinutes(s.MinTimeSinceStartOfTrackMinutes)))
                .ForMember(d => d.MinTimeBetweenBreaks, c => c.MapFrom(s => TimeSpan.FromMinutes(s.TimeBetweenBreaksMinutesRange[0])))
                .ForMember(d => d.MaxTimeBetweenBreaks, c => c.MapFrom(s => TimeSpan.FromMinutes(s.TimeBetweenBreaksMinutesRange[1])))
                .ForMember(d => d.MinLength, c => c.MapFrom((s => TimeSpan.FromSeconds(s.LengthSecondsRange[0]))))
                .ForMember(d => d.MaxLength, c => c.MapFrom(s => TimeSpan.FromSeconds(s.LengthSecondsRange[1])))
                .ForMember(d => d.RampLength, c => c.MapFrom(s => TimeSpan.FromSeconds(s.RampLengthSeconds)))
                ;
            CreateMap<BreaksModel, Breaks>()
                .ForMember(d => d.MinTimeSinceStartOfTrackMinutes, c => c.MapFrom(s => s.MinTimeSinceStartOfTrack.TotalMinutes))
                .ForMember(d => d.TimeBetweenBreaksMinutesRange, c => c.MapFrom(s => new[] { s.MinTimeBetweenBreaks.TotalMinutes, s.MaxTimeBetweenBreaks.TotalMinutes }))
                .ForMember(d => d.LengthSecondsRange, c => c.MapFrom(s => new[] { s.MinLength.TotalSeconds, s.MaxLength.TotalSeconds }))
                .ForMember(d => d.RampLengthSeconds, c => c.MapFrom(s => s.RampLength.TotalSeconds))
                ;
            CreateMap<Rises, RisesModel>()
                .ForMember(d => d.EarliestTime, c => c.MapFrom(s => TimeSpan.FromMinutes(s.EarliestTimeMinutes)))
                .ForMember(d => d.LengthEach, c => c.MapFrom(s => TimeSpan.FromSeconds(s.LengthEachSeconds)))
                ;
            CreateMap<RisesModel, Rises>()
                .ForMember(d => d.EarliestTimeMinutes, c => c.MapFrom(s => s.EarliestTime.TotalMinutes))
                .ForMember(d => d.LengthEachSeconds, c => c.MapFrom(s => s.LengthEach.TotalSeconds));

            CreateMap<Sections, SectionsModel>()
                .ForMember(d => d.TotalLength, c => c.MapFrom(s => TimeSpan.FromSeconds(s.SectionLengthSeconds)))
                .ForMember(d => d.MinFeatureLength, c => c.MapFrom(s => TimeSpan.FromSeconds(s.FeatureLengthRangeSeconds[0])))
                .ForMember(d => d.MaxFeatureLength, c => c.MapFrom(s => TimeSpan.FromSeconds(s.FeatureLengthRangeSeconds[1])))
                .ForMember(d => d.MinRampLength, c => c.MapFrom(s => TimeSpan.FromSeconds(s.RampLengthRangeSeconds[0])))
                .ForMember(d => d.MaxRampLength, c => c.MapFrom(s => TimeSpan.FromSeconds(s.RampLengthRangeSeconds[1])))
                ;

            CreateMap<SectionsModel, Sections>()
                .ForMember(d => d.SectionLengthSeconds, c => c.MapFrom(s => s.TotalLength.TotalSeconds))
                .ForMember(d => d.FeatureLengthRangeSeconds, c => c.MapFrom(s => new[] { s.MinFeatureLength.TotalSeconds, s.MaxFeatureLength.TotalSeconds }))
                .ForMember(d => d.RampLengthRangeSeconds, c => c.MapFrom(s => new[] { s.MinRampLength.TotalSeconds, s.MaxRampLength.TotalSeconds }))
                ;

            CreateMap<ChannelSettings, ChannelSettingsModel>();
            CreateMap<ChannelSettingsModel, ChannelSettings>()
                .ForMember(d => d.UseCustomWaveformExpression, c => c.MapFrom(s => !string.IsNullOrEmpty(s.WaveformExpression)))
                ;

            CreateMap<CreateFileRequest, Settings>()
                .ForMember(d => d.ChannelSettings, c => c.MapFrom(s => s.DualChannel ? new[] { s.Channel0, s.Channel1 } : new[] { s.Channel0 }))
                .ForMember(d => d.Naming, c => c.Ignore())
                .ForMember(d => d.TrackLength, c => c.MapFrom(s => TimeSpan.FromMinutes(s.TrackLengthMinutes)))
                .ForMember(d => d.NumberOfChannels, c => c.MapFrom(s => 2))
                .ForMember(d => d.Version, c => c.MapFrom(s => 1))
                ;

            CreateMap<Settings, CreateFileRequest>()
                .ForMember(d => d.DualChannel, c => c.MapFrom(s => s.ChannelSettings.Length > 1))
                .ForMember(d => d.Channel0, c => c.MapFrom(s => s.ChannelSettings.FirstOrDefault()))
                .ForMember(d => d.Channel1, c => c.MapFrom(s => s.ChannelSettings.Skip(1).FirstOrDefault())) // null if only one channel
                .ForMember(d => d.RecaptchaToken, c => c.Ignore())
                ;
        }   
    }
}
