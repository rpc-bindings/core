import { ObjectDescriptor, MethodDescriptor } from './ObjectDescriptor';
import { FunctionBinder } from './FunctionBinder';
import { Registry } from './Registry';
import { SavedCall } from './SavedCall';
import { BindingConnection } from './BindingConnection';
import { MethodResult, CallbackExecution } from './RpcRequest';

export class ObjectBinder {
    private readonly functions = new Map<number, FunctionBinder>();

    constructor(private objectDescriptor: ObjectDescriptor,
         connection: BindingConnection,
         functionCallRegistry: Registry<SavedCall>,
         callbackRegistry: Registry<(...args:any[]) => any>) {
             
        this.objectDescriptor.methods.forEach(function(f: MethodDescriptor) {
            this.functions.set(f.id, new FunctionBinder(objectDescriptor.id, f, functionCallRegistry, callbackRegistry, connection));
        }.bind(this));
    }

    bind(rootObject: any): void {
        rootObject[this.objectDescriptor.name] = {};

        this.functions.forEach(e => e.bind(rootObject[this.objectDescriptor.name]));
    }
}