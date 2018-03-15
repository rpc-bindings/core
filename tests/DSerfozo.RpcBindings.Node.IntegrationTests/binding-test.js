exports.testMethod1 = async function (callback, input) {
    const result = await test.testMethod1(input);
    callback(null, result);
}

exports.testMethod2 = async function (callback, input) {
    const result = await test.testMethod2(input);
    callback(null, result);
}

exports.dynamic = async function (callback, input) {
    try {
        const obj = await requireObject('testObj');
        const result = await obj.testMethod1(input);

        callback(null, result);
    } catch (e) {
        callback(e.toString, null);
    }
}

exports.delegateTest = async function (callback) {
    const result = await test.delegateTest(function (input) {
        return input + "->JS";
    });

    callback(null, result);
}

exports.testProp = async function (callback, input) {
    try {
        test.testProp = input + "1";
        test.testProp = input + "2";

        const getResult = await test.testProp;

        callback(null, getResult);
    } catch (e) {
        callback(e.toString(), null);
    }
}