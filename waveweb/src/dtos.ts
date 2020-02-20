/* Options:
Date: 2020-02-20 17:23:31
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

export class HelloResponse
{
    public result: string;

    public constructor(init?: Partial<HelloResponse>) { (Object as any).assign(this, init); }
}

// @Route("/createfile")
export class CreateFileRequest implements IReturn<CreateFileRequest>
{
    public randomization: boolean;
    public trackLengthMinutes: number;
    public dualChannel: boolean;
    public phaseShiftCarrier: boolean;
    public phaseShiftPulses: boolean;
    public left: CreateFileRequestChannelSettings;
    public right: CreateFileRequestChannelSettings;

    public constructor(init?: Partial<CreateFileRequest>) { (Object as any).assign(this, init); }
    public createResponse() { return new CreateFileRequest(); }
    public getTypeName() { return 'CreateFileRequest'; }
}

// @Route("/hello")
// @Route("/hello/{Name}")
export class Hello implements IReturn<HelloResponse>
{
    public theNumber: number;
    public name: string;

    public constructor(init?: Partial<Hello>) { (Object as any).assign(this, init); }
    public createResponse() { return new HelloResponse(); }
    public getTypeName() { return 'Hello'; }
}

