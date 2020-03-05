export default class JobProgressModel {
    public jobId: string | null;
    public constructor(init?: Partial<JobProgressModel>) { (Object as any).assign(this, init); }
}
