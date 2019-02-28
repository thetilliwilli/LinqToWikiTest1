const { Sort, ToAttributedProperty } = require("./util.js");
const { Parser } = require("sparqljs");
const fs = require("fs");
const { prefixes } = require("./prefixes");


const parser = new Parser(prefixes);

var inputString = process.argv[2];
inputString = fs.readFileSync("./query1_explicit_select.rq").toString();

var separatorSymbol = "_";
var bindingPrefix = "?";
var labelAppendix = "Label";
var idAppendix = "Id";
var disallowedNames = /[^a-zA-Z_]/;
var discardLabelVariable = s=>s.slice(-"Label".length) !== "Label";

var queryAst = parser.parse(inputString);

var result = queryAst.variables
    .map(s=>s.slice(1))
    .filter(discardLabelVariable)
    .validate(console.log)
    .sort(Sort)
    .validate(function InvalidParameterCountRule(arr) { return arr.length < 1 })
    .validateEach(function DisallowedParameterNameRule(s) { return disallowedNames.test(s) }) //validation check
    .map(s => ({ bindName: s, name: s.split(separatorSymbol).map(s => s[0].toUpperCase() + s.slice(1)).join("") }))//replace "_" char with nothing and make next character upper cased. saved initial names aside with new values
    .mut(arr => [
        arr.map(s => [s.bindName + labelAppendix, s.name]).map(ToAttributedProperty).flat(),
        arr.map(s => [s.bindName, s.name + idAppendix]).map(ToAttributedProperty).flat()
    ])
    .validate(function InvalidPropertyCountRule(arr) { return ((arr[0].length !== arr[1].length) || arr[0].length % 2 !== 0) }) //validation check
    .flat()
    .join("\n")
    ;

console.log(result.length);
console.log(result);