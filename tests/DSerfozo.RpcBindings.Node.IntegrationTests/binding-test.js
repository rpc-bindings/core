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