const { IsNotLabelVariable, AddLabel, Sort, ToAttributedProperty } = require("./util");
var separatorSymbol = "_";
var idAppendix = "Id";
var disallowedNames = /[^a-zA-Z_]/;

function AddId(variable) { return variable + idAppendix; }

function GenerateCsClass(className, variables) {
    var propertiesDeclaration = GeneratePropertiesDeclaration(variables);

    var classDeclaration = GenerateClassDeclaration(className, propertiesDeclaration);
    return classDeclaration;
}

function GeneratePropertiesDeclaration(variables) {
    function LocalShortener(n) { return variables.find(v=>v.name===n).short; };
    var result = variables
        .map(v => v.name)
        .filter(IsNotLabelVariable)
        .sort(Sort)
        .validate(function InvalidParameterCountRule(arr) { return arr.length < 1 })
        .validateEach(function DisallowedParameterNameRule(s) { return disallowedNames.test(s) }) //validation check
        .map(s => ({ bindName: LocalShortener(s), name: s.split(separatorSymbol).map(s => s[0].toUpperCase() + s.slice(1)).join("") }))//replace "_" char with nothing and make next character upper cased. saved initial names aside with new values
        .mut(arr => [
            arr.map(s => [AddLabel(s.bindName), s.name]).map(ToAttributedProperty).flat(),
            arr.map(s => [s.bindName, AddId(s.name)]).map(ToAttributedProperty).flat()
        ])
        .validate(function InvalidPropertyCountRule(arr) { return ((arr[0].length !== arr[1].length) || arr[0].length % 2 !== 0) }) //validation check
        .flat()
        .join("\n")
        ;

    return result;
    // console.log(result.length);
    // console.log(result);
}


function GenerateClassDeclaration(className, propertiesDeclaration) {
    var result =
        `
public class ${className}
{
${propertiesDeclaration}
}
`;

    return result;
}

module.exports = {
    GenerateCsClass,
};