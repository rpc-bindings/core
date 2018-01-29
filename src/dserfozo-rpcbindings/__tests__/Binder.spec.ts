import 'jest';
import { Binder, BindingConnection  } from '../src';
import { EventEmitter } from 'events';

describe('Binder', () => {
    it('Should be created', () => {
        expect(typeof new Binder(<any>{on: function(){}}, new Map<number, any>())).toBe('object');
    });

    it('Should do method execution', async () => {
        const connection = <any>(new EventEmitter());
        let msg;
        connection.sendMethodExecution = function(message) {
            msg = message;
        };
        const binder = new Binder(<any>connection, new Map<number, any>([[1, {id:1, name:'test', methods:[{id:3, name:'testFunc'}]}]]));
        const obj = {};

        binder.bind(obj);

        const promise = obj.test.testFunc();

        connection.emit(BindingConnection.METHOD_RESULT, {executionId: msg.executionId, success: true, result: 'test'});

        await expect(promise).resolves.toEqual('test');
    });

    it('Should execute sync callback', async () => {
        const connection = <any>(new EventEmitter());
        let msg;
        connection.sendMethodExecution = function(message) {
            msg = message;
        };
        connection.sendCallbackResult = function() {};
        const binder = new Binder(<any>connection, new Map<number, any>([[1, {id:1, name:'test', methods:[{id:3, name:'testFunc'}]}]]));
        const obj = {};

        binder.bind(obj);

        const callback = jest.fn().mockReturnValue(1);
        const promise = obj.test.testFunc(callback);

        connection.emit(BindingConnection.METHOD_RESULT, {executionId: msg.executionId, success: true, result: 'test'});

        await promise;

        connection.emit(BindingConnection.CALLBACK_EXECUTION, {functionId: msg.parameters[0].functionId, executionId: 2, parameters: ['text', 1]})

        expect(callback).toBeCalledWith('text', 1);
    });

    it('Should send sync callback result', async () => {
        const connection = <any>(new EventEmitter());
        let msg;
        connection.sendMethodExecution = function(message) {
            msg = message;
        };
        connection.sendCallbackResult = jest.fn();
        const binder = new Binder(<any>connection, new Map<number, any>([[1, {id:1, name:'test', methods:[{id:3, name:'testFunc'}]}]]));
        const obj = {};

        binder.bind(obj);

        const promise = obj.test.testFunc(function(){return 'result'});

        connection.emit(BindingConnection.METHOD_RESULT, {executionId: msg.executionId, success: true, result: 'test'});

        await promise;

        connection.emit(BindingConnection.CALLBACK_EXECUTION, {functionId: msg.parameters[0].functionId, executionId: 2, parameters: ['text', 1]})

        expect(connection.sendCallbackResult).toBeCalledWith({executionId: 2, success: true, result: 'result'});
    });

    it('Should send sync callback error', async () => {
        const connection = <any>(new EventEmitter());
        let msg;
        connection.sendMethodExecution = function(message) {
            msg = message;
        };
        connection.sendCallbackResult = jest.fn();
        const binder = new Binder(<any>connection, new Map<number, any>([[1, {id:1, name:'test', methods:[{id:3, name:'testFunc'}]}]]));
        const obj = {};

        binder.bind(obj);

        obj.test.testFunc(function(){throw 'exception';});

        connection.emit(BindingConnection.METHOD_RESULT, {executionId: msg.executionId, success: true, result: 'test'});

        connection.emit(BindingConnection.CALLBACK_EXECUTION, {functionId: msg.parameters[0].functionId, executionId: 2, parameters: ['text', 1]})

        expect(connection.sendCallbackResult).toBeCalledWith({executionId: 2, success: false, exception: 'exception'});
    });

    it('Should send async callback result', async () => {
        const connection = <any>(new EventEmitter());
        let msg;
        connection.sendMethodExecution = function(message) {
            msg = message;
        };
        connection.sendCallbackResult = jest.fn();
        const binder = new Binder(<any>connection, new Map<number, any>([[1, {id:1, name:'test', methods:[{id:3, name:'testFunc'}]}]]));
        const obj = {};

        binder.bind(obj);

        const callbackPromise = Promise.resolve('result');
        const promise = obj.test.testFunc(function(){return callbackPromise;});

        connection.emit(BindingConnection.METHOD_RESULT, {executionId: msg.executionId, success: true, result: 'test'});

        await promise;

        connection.emit(BindingConnection.CALLBACK_EXECUTION, {functionId: msg.parameters[0].functionId, executionId: 2, parameters: ['text', 1]})

        await callbackPromise;

        expect(connection.sendCallbackResult).toBeCalledWith({executionId: 2, success: true, result: 'result'});
    });

    it('Should send async callback error', async () => {
        const connection = <any>(new EventEmitter());
        let msg;
        connection.sendMethodExecution = function(message) {
            msg = message;
        };
        connection.sendCallbackResult = jest.fn();
        const binder = new Binder(<any>connection, new Map<number, any>([[1, {id:1, name:'test', methods:[{id:3, name:'testFunc'}]}]]));
        const obj = {};

        binder.bind(obj);

        const callbackPromise = Promise.reject('exception');
        obj.test.testFunc(function(){return callbackPromise;});

        connection.emit(BindingConnection.METHOD_RESULT, {executionId: msg.executionId, success: true, result: 'test'});
        connection.emit(BindingConnection.CALLBACK_EXECUTION, {functionId: msg.parameters[0].functionId, executionId: 2, parameters: ['text', 1]})

        try {
            await callbackPromise;
        }catch {

        }

        expect(connection.sendCallbackResult).toBeCalledWith({executionId: 2, success: false, exception: 'exception'});
    });

    it('Should delete callback', async () => {
        const connection = <any>(new EventEmitter());
        let msg;
        connection.sendMethodExecution = function(message) {
            msg = message;
        };
        connection.sendCallbackResult = jest.fn();
        const binder = new Binder(<any>connection, new Map<number, any>([[1, {id:1, name:'test', methods:[{id:3, name:'testFunc'}]}]]));
        const obj = {};

        binder.bind(obj);

        const callback = jest.fn();
        const promise = obj.test.testFunc(callback);

        connection.emit(BindingConnection.METHOD_RESULT, {executionId: msg.executionId, success: true, result: 'test'});

        await promise;

        connection.emit(BindingConnection.DELETE_CALLBACK, {functionId: msg.parameters[0].functionId })
        connection.emit(BindingConnection.CALLBACK_EXECUTION, {functionId: msg.parameters[0].functionId, executionId: 2, parameters: ['text', 1]})

        expect(callback).not.toBeCalled();
    });
});