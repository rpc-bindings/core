exports.testMethod1 = async function (callback, input) {
    const result = await test.testMethod1(input);
    callback(null, result);
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