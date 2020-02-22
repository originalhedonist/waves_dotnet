/* Options:
Date: 2020-02-22 19:56:42
Version: 5.81
Tip: To override a DTO option, remove "//" prefix before updating
BaseUrl: https://localhost:5001

//GlobalNamespace: 
//AddServiceStackTypes: True
//AddResponseStatus: False
//AddImplicitVersion: 
//AddDescriptionAsComments: True
//IncludeTypes: 
//ExcludeTypes: 
//DefaultImports: 
*/


export interface IReturn<T>
{
    createResponse(): T;
}

export interface IReturnVoid
{
    createResponse(): void;
}

export class Variance
{
    public randomness: number;
    public progression: number;

    public constructor(init?: Partial<Variance>) { (Object as any).assign(this, init); }
}

export class Sections
{
    public sectionLengthSeconds: number;
    public minFeatureLengthSeconds: number;
    public maxFeatureLengthSeconds: number;
    public featureLengthVariation: Variance;
    public minRampLengthSeconds: number;
    public maxRampLengthSeconds: number;
    public rampLengthVariation: Variance;

    public constructor(init?: Partial<Sections>) { (Object as any).assign(this, init); }
}

export class FeatureProbability
{
    public frequencyWeighting: number;
    public wetnessWeighting: number;
    public nothingWeighting: number;

    public constructor(init?: Partial<FeatureProbability>) { (Object as any).assign(this, init); }
}

export class CarrierFrequency
{
    public left: string;
    public right: string;

    public constructor(init?: Partial<CarrierFrequency>) { (Object as any).assign(this, init); }
}

export class PulseFrequency
{
    public quiescent: number;
    public low: number;
    public high: number;
    public chanceOfHigh: number;
    public variation: Variance;

    public constructor(init?: Partial<PulseFrequency>) { (Object as any).assign(this, init); }
}

export class Wetness
{
    public linkToFeature: boolean;
    public minimum: number;
    public maximum: number;
    public variation: Variance;

    public constructor(init?: Partial<Wetness>) { (Object as any).assign(this, init); }
}

export class Breaks
{
    public minTimeSinceStartOfTrackMinutes: number;
    public minTimeBetweenBreaksMinutes: number;
    public maxTimeBetweenBreaksMinutes: number;
    public minLengthSeconds: number;
    public maxLengthSeconds: number;
    public rampLengthSeconds: number;

    public constructor(init?: Partial<Breaks>) { (Object as any).assign(this, init); }
}

export class Rises
{
    public count: number;
    public earliestTimeMinutes: number;
    public lengthEachSeconds: number;
    public amount: number;

    public constructor(init?: Partial<Rises>) { (Object as any).assign(this, init); }
}

export class ChannelSettings
{
    public useCustomWaveformExpression: boolean;
    public waveformExpression: string;
    public sections: Sections;
    public featureProbability: FeatureProbability;
    public carrierFrequency: CarrierFrequency;
    public pulseFrequency: PulseFrequency;
    public wetness: Wetness;
    public breaks: Breaks;
    public rises: Rises;

    public constructor(init?: Partial<ChannelSettings>) { (Object as any).assign(this, init); }
}

// @Route("/createfile")
export class CreateFileRequest implements IReturn<CreateFileRequest>
{
    public randomization: boolean;
    public trackLengthMinutes: number;
    public dualChannel: boolean;
    public phaseShiftCarrier: boolean;
    public phaseShiftPulses: boolean;
    public channel0: ChannelSettings;
    public channel1: ChannelSettings;

    public constructor(init?: Partial<CreateFileRequest>) { (Object as any).assign(this, init); }
    public createResponse() { return new CreateFileRequest(); }
    public getTypeName() { return 'CreateFileRequest'; }
}

