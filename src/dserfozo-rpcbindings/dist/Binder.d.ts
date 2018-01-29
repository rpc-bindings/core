import { BindingConnection } from './BindingConnection';
import { ObjectDescriptor } from './ObjectDescriptor';
export declare class Binder {
    private connection;
    private readonly callRegistry;
    private readonly callbackRegistry;
    private readonly boundObjects;
    constructor(connection: BindingConnection, objectDescriptors: Map<number, ObjectDescriptor>);
    bind(rootObject: any): void;
    private onDeleteCallback(deleteCallback);
    private onCallbackExecution(execution);
    private onMethodResult(result);
}
