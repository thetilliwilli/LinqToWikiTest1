Array.prototype.mut = function(converter){return converter(this);}
Array.prototype.validate = function(validator){if(validator(this))throw new Error(`Validator violation: ${validator.name}`);return this;}
Array.prototype.validateEach = function(validator){if(this.find(validator))throw new Error(`Validator violation: ${validator.name}`);return this;}

function Sort(a,b){return a.localeCompare(b);}
function ToAttributedProperty([attribute, property]){return [`[JsonProperty("${attribute}")]`, `public string ${property};`];}

module.exports = {
    Sort,
    ToAttributedProperty,
};