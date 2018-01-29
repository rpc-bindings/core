import { MethodDescriptor } from './ObjectDescriptor';
import { Registry } from './Registry';
import { SavedCall } from './SavedCall';
import { BindingConnection } from './BindingConnection';
export declare class FunctionBinder {
    private objectId;
    private descriptor;
    private functionCallRegistry;
    private callbackRegistry;
    private connection;
    constructor(objectId: number, descriptor: MethodDescriptor, functionCallRegistry: Registry<SavedCall>, callbackRegistry: Registry<(...args: any[]) => any>, connection: BindingConnection);
    bind(obj: any): void;
}
