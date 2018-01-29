var rpcbindings = require('dserfozo-rpcbindings');

let binder;
exports.initialize = function(callback, bindings) {
    const stream = callback.stream;
    
    binder = new rpcbindings.Binder(new rpcbindings.BindingConnection(stream), rpcbindings.buildMapIntKeyRecursive(bindings, 'methods'));
    binder.bind(global);
}