import { ObjectDescriptor, MethodDescriptor } from './ObjectDescriptor';
import { FunctionBinder } from './FunctionBinder';
import { Registry } from './Registry';
import { SavedCall } from './SavedCall';
import { BindingConnection } from './BindingConnection';
import { MethodResult, CallbackExecution } from './RpcRequest';
import { PropertyBinder } from './PropertyBinder';

export class ObjectBinder {
    private readonly functions = new Map<number, FunctionBinder>();
    private readonly properties = new Map<number, PropertyBinder>();

    constructor(private objectDescriptor: ObjectDescriptor,
        connection: BindingConnection,
        functionCallRegistry: Registry<SavedCall>,
        propertyExecutionRegistry: Registry<SavedCall>,
        callbackRegistry: Registry<(...args: any[]) => any>) {

        if (this.objectDescriptor.methods) {
            this.objectDescriptor.methods.forEach(function (f: MethodDescriptor) {
                this.functions.set(f.id, new FunctionBinder(objectDescriptor.id, f, functionCallRegistry, callbackRegistry, connection));
            }.bind(this));
        }

        if (this.objectDescriptor.properties) {
            this.objectDescriptor.properties
            .forEach(p => this.properties.set(p.id, 
                new PropertyBinder(objectDescriptor.id, p, propertyExecutionRegistry, callbackRegistry, connection)));
        }
    }

    bind(rootObject: any): void {
        const namespace = this.objectDescriptor.name.split('.');

        let context = rootObject;
        for (var i = 0; i < namespace.length; i++) {
            context = context[namespace[i]] ? context[namespace[i]] : (context[namespace[i]] = {});
        }

        this.functions.forEach(e => e.bind(context));
        this.properties.forEach(e => e.bind(context));
    }
}