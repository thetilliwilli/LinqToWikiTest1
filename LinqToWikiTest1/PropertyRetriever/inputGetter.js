const stdinReader = require("./stdin-reader");
const fs = require("fs");

var inputStringFromEnviromentArg = process.argv[2];
// inputStringFromEnviromentArg = fs.readFileSync("./query4_ex1.rq").toString();

function getInputString() {
    if (inputStringFromEnviromentArg === undefined)
        return Promise.resolve(stdinReader.read(process.stdin));
    else
        return Promise.resolve(inputStringFromEnviromentArg);
}

module.exports = {
    getInputString,
};