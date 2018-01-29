var t = require('testmodule');

async function doIt(callback) {
    //const result = await bound.doIt();
    //const numberResult = await bound.add(9, 1);
    //await bound.callbackTest(function (data) {
    //    console.log('hello world callback: ' + data);
    //});

    //console.log('NUMBERS: ' + numberResult);

    if (t) {
        callback(null, /*result + ' ' + */t.do(1, 2));
    } else {
        callback(null, '');
    }
}

exports.doIt = doIt;