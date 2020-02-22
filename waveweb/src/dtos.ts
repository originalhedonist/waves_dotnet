/* Options:
Date: 2020-02-22 14:56:26
Version: 5.80
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

export class CreateFileRequestVariance
{
    public randomness: number;
    public progression: number;

    public constructor(init?: Partial<CreateFileRequestVariance>) { (Object as any).assign(this, init); }
}

export class CreateFileRequestChannelSettings
{
    public waveformExpression: string;
    public sectionLengthSeconds: number;
    public minFeatureLengthSeconds: number;
    public maxFeatureLengthSeconds: number;
    public featureLengthVariation: CreateFileRequestVariance;

    public constructor(init?: Partial<CreateFileRequestChannelSettings>) { (Object as any).assign(this, init); }
}

// @Route("/createfile")
export class CreateFileRequest implements IReturn<CreateFileRequest>
{
    public randomization: boolean;
    public trackLengthMinutes: number;
    public dualChannel: boolean;
    public phaseShiftCarrier: boolean;
    public phaseShiftPulses: boolean;
    public channel0: CreateFileRequestChannelSettings;
    public channel1: CreateFileRequestChannelSettings;

    public constructor(init?: Partial<CreateFileRequest>) { (Object as any).assign(this, init); }
    public createResponse() { return new CreateFileRequest(); }
    public getTypeName() { return 'CreateFileRequest'; }
}

