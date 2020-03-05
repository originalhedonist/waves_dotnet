using AutoMapper;
using Ultimate.DI;
using wavegenerator.library;
using wavegenerator.models;
using waveweb.ServiceInterface;
using waveweb.ServiceModel;

namespace waveweb
{
    public class WaveformTestPulseGeneratorProvider : IWaveformTestPulseGeneratorProvider
    {
        private readonly IContainer container;
        private readonly IMapper mapper;

        public WaveformTestPulseGeneratorProvider(Ultimate.DI.IContainer container, IMapper mapper)
        {
            this.container = container;
            this.mapper = mapper;
        }

        public PulseGenerator GetPulseGenerator(TestPulseWaveformRequest testPulseWaveformRequest, GetPulseGeneratorParams parameters)
        {
            var pulseFrequencyModel = mapper.Map<PulseFrequency, PulseFrequencyModel>(testPulseWaveformRequest.PulseFrequency);
            var sectionsModel = mapper.Map<Sections, SectionsModel>(testPulseWaveformRequest.Sections);
            var waveformExpressionProvider = new WaveformExpressionProvider(testPulseWaveformRequest.WaveformExpression);
            var waveFileMetadata = new WavefileMetadata(numberOfChannels: 1, phaseShiftCarrier: false, phaseShiftPulses: false, randomization: false, trackLengthSeconds: testPulseWaveformRequest.Sections.SectionLengthSeconds);
            var featureChooser = new AlwaysFeatureChooser(parameters.ChooseFeature);
            var samplingFrequencyProvider = new SamplingFrequencyProvider(parameters.SamplingFrequency);

            // this composes a subset of the main file-creation component stack
            // (DependencyConfig.Configure(settings) creates the full stack)
            var nestedContainer = container.GetNestedContainer();
            nestedContainer.AddInstance(sectionsModel);
            nestedContainer.AddInstance(pulseFrequencyModel);
            nestedContainer.AddInstance<IWaveformExpressionProvider>(waveformExpressionProvider);
            nestedContainer.AddInstance<IWaveFileMetadata>(waveFileMetadata);
            nestedContainer.AddInstance<IFeatureChooser>(featureChooser);
            nestedContainer.AddInstance<ISamplingFrequencyProvider>(samplingFrequencyProvider);
            var pulseGenerator = nestedContainer.Resolve<PulseGenerator>();
            return pulseGenerator;
        }
    }

}