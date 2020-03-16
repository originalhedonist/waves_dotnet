export default interface GRecaptchaObject {
    execute(siteKey: string, options: any): PromiseLike<string>;
}