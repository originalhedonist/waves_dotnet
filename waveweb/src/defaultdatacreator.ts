import '@/dtos';
import { ChannelSettings, Sections, Variance, FeatureProbability, PulseFrequency, CarrierFrequency, Breaks, Rises, Wetness } from '@/dtos';

export default class DefaultDataCreator {
    public static createDefaultChannelSettings(): ChannelSettings {
        return new ChannelSettings({
            sections: new Sections({
                sectionLengthSeconds: 30,
                featureLengthRangeSeconds: [10, 20],
                rampLengthRangeSeconds: [2, 5],
                rampLengthVariation: new Variance({
                    progression: 0.7,
                    randomness: 0.3,
                }),
                featureLengthVariation: new Variance({
                    progression: 0.7,
                    randomness: 0.3,
                }),
            }),
            featureProbability: new FeatureProbability({
                frequencyWeighting: 1.0,
                wetnessWeighting: 0.2,
                nothingWeighting: 0.8,
            }),
            pulseFrequency: new PulseFrequency({
                quiescent: 0.8,
                low: 0.4,
                high: 2.0,
                chanceOfHigh: 0.6,
                variation: new Variance({
                    progression: 0.7,
                    randomness: 0.3,
                }),
            }),
            carrierFrequency: new CarrierFrequency({
                left: '800',
                right: '800',
            }),
            breaks: new Breaks({
                lengthSecondsRange: [10, 60],
                minTimeSinceStartOfTrackMinutes: 10,
                timeBetweenBreaksMinutesRange: [2, 20],
                rampLengthSeconds: 20,
            }),
            rises: new Rises({
                count: 2,
                earliestTimeMinutes: 10,
                amount: 0.08,
                lengthEachSeconds: 20,
            }),
            wetness: new Wetness({
                amountRange: [0.4, 0.6],
                linkToFeature: true,
                variation: new Variance({
                    progression: 0.7,
                    randomness: 0.3,
                }),
            }),
        });
    }
}
