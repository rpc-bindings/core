import { ObjectDescriptor } from './ObjectDescriptor';
import { Registry } from './Registry';
import { SavedCall } from './SavedCall';
import { BindingConnection } from './BindingConnection';
export declare class ObjectBinder {
    private objectDescriptor;
    private readonly functions;
    constructor(objectDescriptor: ObjectDescriptor, connection: BindingConnection, functionCallRegistry: Registry<SavedCall>, callbackRegistry: Registry<(...args: any[]) => any>);
    bind(rootObject: any): void;
}
