using System;
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
            CreateMap<CarrierFrequency, CarrierFrequencyModel>();
            CreateMap<PulseFrequency, PulseFrequencyModel>();
            CreateMap<Variance, VarianceModel>();
            CreateMap<Wetness, WetnessModel>()
                .ForMember(d => d.Minimum, c => c.MapFrom(s => s.AmountRange[0]))
                .ForMember(d => d.Maximum, c => c.MapFrom(s => s.AmountRange[1]))
                ;
            CreateMap<Breaks, BreaksModel>()
                .ForMember(d => d.MinTimeSinceStartOfTrack, c => c.MapFrom(s => TimeSpan.FromMinutes(s.MinTimeSinceStartOfTrackMinutes)))
                .ForMember(d => d.MinTimeBetweenBreaks, c => c.MapFrom(s => TimeSpan.FromMinutes(s.TimeBetweenBreaksMinutesRange[0])))
                .ForMember(d => d.MaxTimeBetweenBreaks, c => c.MapFrom(s => TimeSpan.FromMinutes(s.TimeBetweenBreaksMinutesRange[1])))
                .ForMember(d => d.MinLength, c => c.MapFrom((s => TimeSpan.FromSeconds(s.LengthSecondsRange[0]))))
                .ForMember(d => d.MaxLength, c => c.MapFrom(s => TimeSpan.FromSeconds(s.LengthSecondsRange[1])))
                .ForMember(d => d.RampLength, c => c.MapFrom(s => TimeSpan.FromSeconds(s.RampLengthSeconds)))
                ;
            CreateMap<Rises, RisesModel>()
                .ForMember(d => d.EarliestTime, c => c.MapFrom(s => TimeSpan.FromMinutes(s.EarliestTimeMinutes)))
                .ForMember(d => d.LengthEach, c => c.MapFrom(s => TimeSpan.FromSeconds(s.LengthEachSeconds)))
                ;
            CreateMap<Sections, SectionsModel>()
                .ForMember(d => d.TotalLength, c => c.MapFrom(s => TimeSpan.FromSeconds(s.SectionLengthSeconds)))
                .ForMember(d => d.MinFeatureLength, c => c.MapFrom(s => TimeSpan.FromSeconds(s.FeatureLengthRangeSeconds[0])))
                .ForMember(d => d.MaxFeatureLength, c => c.MapFrom(s => TimeSpan.FromSeconds(s.FeatureLengthRangeSeconds[1])))
                .ForMember(d => d.MinRampLength, c => c.MapFrom(s => TimeSpan.FromSeconds(s.RampLengthRangeSeconds[0])))
                .ForMember(d => d.MaxRampLength, c => c.MapFrom(s => TimeSpan.FromSeconds(s.RampLengthRangeSeconds[1])))
                ;
            CreateMap<ChannelSettings, ChannelSettingsModel>();

            CreateMap<CreateFileRequest, Settings>()
                .ForMember(d => d.ChannelSettings, c => c.MapFrom(s => s.DualChannel ? new[] { s.Channel0, s.Channel1 } : new[] { s.Channel0 }))
                .ForMember(d => d.ConvertToMp3, c => c.MapFrom(s => false))
                .ForMember(d => d.NumFiles, c => c.MapFrom(s => 1))
                .ForMember(d => d.Naming, c => c.Ignore())
                .ForMember(d => d.TrackLength, c => c.MapFrom(s => TimeSpan.FromMinutes(s.TrackLengthMinutes)))
                .ForMember(d => d.NumberOfChannels, c => c.MapFrom(s => 2))
                ;
        }
    }
}
