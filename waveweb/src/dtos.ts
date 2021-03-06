/* Options:
Date: 2020-03-16 13:00:16
Version: 5.81
Tip: To override a DTO option, remove "//" prefix before updating
BaseUrl: https://localhost:5001

//GlobalNamespace: 
//AddServiceStackTypes: True
AddResponseStatus: True
//AddImplicitVersion: 
//AddDescriptionAsComments: True
//IncludeTypes: 
//ExcludeTypes: 
DefaultImports: ResponseStatus:@servicestack/client
*/

import { ResponseStatus } from '@servicestack/client';

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
    public featureLengthRangeSeconds: number[];
    public featureLengthVariation: Variance;
    public rampLengthRangeSeconds: number[];
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
    public amountRange: number[];
    public variation: Variance;

    public constructor(init?: Partial<Wetness>) { (Object as any).assign(this, init); }
}

export class Breaks
{
    public minTimeSinceStartOfTrackMinutes: number;
    public timeBetweenBreaksMinutesRange: number[];
    public lengthSecondsRange: number[];
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

export enum JobProgressStatus
{
    Complete = 'Complete',
    InProgress = 'InProgress',
    Failed = 'Failed',
}

export class CreateFileResponse
{
    public jobId: string;
    public responseStatus: ResponseStatus;

    public constructor(init?: Partial<CreateFileResponse>) { (Object as any).assign(this, init); }
}

export class DownloadSettingsResponse
{
    public downloadId: string;
    public responseStatus: ResponseStatus;

    public constructor(init?: Partial<DownloadSettingsResponse>) { (Object as any).assign(this, init); }
}

export class JobProgress
{
    public status: JobProgressStatus;
    public progress: number;
    public message: string;
    public responseStatus: ResponseStatus;

    public constructor(init?: Partial<JobProgress>) { (Object as any).assign(this, init); }
}

export class TestPulseWaveformResponse
{
    public success: boolean;
    public errorMessage: string;
    public sampleNoFeature: number[][];
    public sampleHighFrequency: number[][];
    public sampleLowFrequency: number[][];
    public responseStatus: ResponseStatus;

    public constructor(init?: Partial<TestPulseWaveformResponse>) { (Object as any).assign(this, init); }
}

// @Route("/createfile")
export class CreateFileRequest implements IReturn<CreateFileResponse>
{
    public randomization: boolean;
    public trackLengthMinutes: number;
    public dualChannel: boolean;
    public phaseShiftCarrier: boolean;
    public phaseShiftPulses: boolean;
    public channel0: ChannelSettings;
    public channel1: ChannelSettings;
    public recaptchaToken: string;

    public constructor(init?: Partial<CreateFileRequest>) { (Object as any).assign(this, init); }
    public createResponse() { return new CreateFileResponse(); }
    public getTypeName() { return 'CreateFileRequest'; }
}

export class UploadSettingsResponse
{
    public request: CreateFileRequest;
    public responseStatus: ResponseStatus;

    public constructor(init?: Partial<UploadSettingsResponse>) { (Object as any).assign(this, init); }
}

// @Route("/downloadfile/{Id}")
export class DownloadFileRequest implements IReturn<Blob>
{
    public id: string;

    public constructor(init?: Partial<DownloadFileRequest>) { (Object as any).assign(this, init); }
    public createResponse() { return new Blob(); }
    public getTypeName() { return 'DownloadFileRequest'; }
}

// @Route("/downloadsettings")
export class DownloadSettingsRequest implements IReturn<DownloadSettingsResponse>
{
    public request: CreateFileRequest;

    public constructor(init?: Partial<DownloadSettingsRequest>) { (Object as any).assign(this, init); }
    public createResponse() { return new DownloadSettingsResponse(); }
    public getTypeName() { return 'DownloadSettingsRequest'; }
}

// @Route("/jobprogress")
export class JobProgressRequest implements IReturn<JobProgress>
{
    public jobId: string;

    public constructor(init?: Partial<JobProgressRequest>) { (Object as any).assign(this, init); }
    public createResponse() { return new JobProgress(); }
    public getTypeName() { return 'JobProgressRequest'; }
}

// @Route("/testpulsewaveform")
export class TestPulseWaveformRequest implements IReturn<TestPulseWaveformResponse>
{
    public waveformExpression: string;
    public sections: Sections;
    public pulseFrequency: PulseFrequency;

    public constructor(init?: Partial<TestPulseWaveformRequest>) { (Object as any).assign(this, init); }
    public createResponse() { return new TestPulseWaveformResponse(); }
    public getTypeName() { return 'TestPulseWaveformRequest'; }
}

// @Route("/uploadsettings")
export class UploadSettingsRequest implements IReturn<UploadSettingsResponse>
{
    public settingsFile: string;

    public constructor(init?: Partial<UploadSettingsRequest>) { (Object as any).assign(this, init); }
    public createResponse() { return new UploadSettingsResponse(); }
    public getTypeName() { return 'UploadSettingsRequest'; }
}

