import { PropertyDescriptor } from './ObjectDescriptor';
import { Registry } from './Registry';
import { SavedCall } from './SavedCall';
import { BindingConnection } from './BindingConnection';

export class PropertyBinder {
    constructor(private objectId: number,
        private descriptor: PropertyDescriptor,
        private propertyExecutionRegistry: Registry<SavedCall>,
        private callbackRegistry: Registry<(...args: any[]) => any>,
        private connection: BindingConnection) {

    }

    bind(obj: any): void {
        const descriptor = this.descriptor;
        const connection = this.connection;
        const objectId = this.objectId;
        const registry = this.propertyExecutionRegistry;
        const callbackRegistry = this.callbackRegistry;

        let lastSetPromise = Promise.resolve();
        let propertyDescriptor = {
            configurable: false,
            enumerable: true,
            get: (function () {
                if (!descriptor.readable) {
                    return Promise.reject('Property is not readable.');
                }

                return lastSetPromise.then(function () {
                    return new Promise(function (resolve, reject) {
                        const executionId = registry.save(new SavedCall(resolve, reject));

                        connection.sendPropertyGetExecution({ executionId: executionId, objectId: objectId, propertyId: descriptor.id });
                    });
                });
            }).bind(this)
        };

        if (descriptor.writable) {
            propertyDescriptor['set'] = (function (value) {
                lastSetPromise = new Promise(function (resolve, reject) {
                    const executionId = registry.save(new SavedCall(resolve, reject));

                    if(typeof value === 'function') {
                        value = { functionId: callbackRegistry.save(value) }
                    }

                    connection.sendPropertySetExecution({executionId: executionId, objectId: objectId, propertyId: descriptor.id, value: value});
                });
            }).bind(this);
        }

        Object.defineProperty(obj, this.descriptor.name, propertyDescriptor);
    }
}