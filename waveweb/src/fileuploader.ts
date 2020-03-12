export default interface FileUploader extends EventTarget {
    files: File[];
    value: string;
}
