var originalLabelAppendix = "Label";
var resultLabelAppendix = "L";
var questionMarkChar = "?";

Array.prototype.mut = function (converter) { return converter(this); }
Array.prototype.validate = function (validator) { if (validator(this)) throw new Error(`Validator violation: ${validator.name}`); return this; }
Array.prototype.validateEach = function (validator) { if (this.find(validator)) throw new Error(`Validator violation: ${validator.name}`); return this; }

function Sort(a, b) { return a.localeCompare(b); }
function ToAttributedProperty([attribute, property]) { return [`[JsonProperty("${attribute}")]`, `public string ${property};`]; }

function AddLabel(variable) { return variable + resultLabelAppendix; }
function AddMark(variable) { return questionMarkChar + variable; }
function IsNotLabelVariable(variable) { return variable.slice(-originalLabelAppendix.length) !== originalLabelAppendix; }
function IsLabelVariable(variable) { return variable.slice(-originalLabelAppendix.length) === originalLabelAppendix; }
function RemoveQuestionMark(variable) { return variable.slice(questionMarkChar.length); }

module.exports = {
    Sort,
    ToAttributedProperty,
    AddLabel,
    AddMark,
    IsNotLabelVariable,
    IsLabelVariable,
    RemoveQuestionMark,

    resultLabelAppendix,
    questionMarkChar,
    originalLabelAppendix,
};