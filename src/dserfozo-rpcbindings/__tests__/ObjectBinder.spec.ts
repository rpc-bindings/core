import 'jest';
import { ObjectBinder, FunctionBinder } from '../src';

describe('ObjectBinder', () => {
    it('Should create', () => {
        expect(typeof new ObjectBinder(<any>{methods:[]}, <any>{}, <any>{}, <any>{})).toBe('object');
    });

    it('Should create bound object', () => {
        const binder = new ObjectBinder(<any>{id:1, name:'objName', methods:[]}, <any>{}, <any>{}, <any>{});
        const obj = {};
        binder.bind(obj);

        expect(typeof obj.objName).toBe('object');
    });

    it('Should create function binder', () => {
        var _bind = Function.prototype.apply.bind(Function.prototype.bind);
        Object.defineProperty(Function.prototype, 'bind', {
            value: function(obj) {
                var boundFunction = _bind(this, arguments);
                boundFunction.boundObject = obj;
                return boundFunction;
            }
        });

        const binder = new ObjectBinder(<any>{id:1, name:'objName', methods:[{id:3, name:'testFunc'}]}, <any>{}, <any>{}, <any>{});
        const obj = {};
        binder.bind(obj);

        expect(obj.objName.testFunc.boundObject).toBeInstanceOf(FunctionBinder);
    });
});