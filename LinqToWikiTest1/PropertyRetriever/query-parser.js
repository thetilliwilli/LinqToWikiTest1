const minify = require("./minify");

var questionMarkChar = "?";
var resultLabelAppendix = "L";

function RemoveDublicates(item, index, array) {
    return array.indexOf(item) === index;
}

function AddLabel(variable) {
    return variable + resultLabelAppendix;
}

function AddLabelFilterClause(variable) {
    return `FILTER(LANG(${variable})="en")`
}

function AddMark(variable) {
    return "?" + variable;
}

function Sort(a, b) { return a.localeCompare(b); }

function IsNotLabelVariable(variable) { return variable.slice(-"Label".length) !== "Label"; }

function RemoveQuestionMark(variable) { return variable.slice(questionMarkChar.length); }

class QueryParser {

    static Parse(inputString) {

        var variables = inputString.match(/\?\w+/img)
            .filter(RemoveDublicates)
            .filter(IsNotLabelVariable)
            .map(RemoveQuestionMark)
            ;

        var shortener = minify.GenerateShortener(variables);
        var body = this.InjectShortNames(inputString.trim(), variables);
        var trimmed = body.toLowerCase();
        if (!trimmed.includes("select") || !trimmed.includes("where"))
            throw new Error("Incorrect query - cant find mandatory SELECT and WHERE clauses");


        var justAfterSelectAnchor = trimmed.indexOf("select") + "select".length;
        var firstParenthesisAfterWhereAnchor = trimmed.indexOf("{", trimmed.indexOf("where") + "where".length);


        var beforeSelectedIncluded = body.slice(0, justAfterSelectAnchor);
        var outVariables = [
            ...variables.map(shortener).map(AddMark),
            ...variables.map(shortener).map(AddMark).map(AddLabel),
        ].join(" ").trim();
        var languageFilters = variables.map(shortener).map(AddMark).map(AddLabel).map(AddLabelFilterClause).join("\n");
        var afterWhereAndFuther = body.slice(firstParenthesisAfterWhereAnchor + 1);
        afterWhereAndFuther = this.InjectOptionalRdfsNames(afterWhereAndFuther);
        afterWhereAndFuther = this.RemoveLabelServiceDeclaration(afterWhereAndFuther);

        var composition = `${beforeSelectedIncluded} ${outVariables} where { \n${languageFilters}\n${afterWhereAndFuther}`
            .replace(/\n/img, " ")
            .replace(/  /img, " ")
            ;

        return {
            original: inputString,
            composition: composition,
            variables: variables,
            shortdic: shortener.shortDic,
        };
    }

    /** OPTIONAL { ?game wdt:P400 ?platform. } -> OPTIONAL { ?game wdt:P400 ?platform. ?platform rdfs:label ?platformLabel. } */
    static InjectOptionalRdfsNames(afterWhereAndFuther) {
        // console.log("afterWhereAndFuther", afterWhereAndFuther);
        var matchesOfOptions = afterWhereAndFuther.match(/OPTIONAL\W[^}]+\}/img);
        if (matchesOfOptions === null)
            return afterWhereAndFuther;
        var firstMatchIndex = afterWhereAndFuther.indexOf(matchesOfOptions[0]);
        var lastMatchIndex = afterWhereAndFuther.lastIndexOf(matchesOfOptions[matchesOfOptions.length - 1]);
        var lastMatchLength = matchesOfOptions[matchesOfOptions.length - 1].length;

        var optionalList = matchesOfOptions;
        var optionalsWithRdfsName = optionalList
            .map(option => {
                var matches = option.match(/(\?\w+)/img);
                if (!matches) throw new Error("no matches in OPTION - its strange");
                var lastMatch = matches.pop();
                var indexOfLastMatch = option.lastIndexOf(lastMatch);
                return `${option.slice(0, indexOfLastMatch)} ${lastMatch}. ${lastMatch} rdfs:label ${AddLabel(lastMatch)}. }`;
            })
            .join("\n")
            ;

        return `${afterWhereAndFuther.slice(0, firstMatchIndex)} ${optionalsWithRdfsName} ${afterWhereAndFuther.slice(lastMatchIndex + lastMatchLength)}`;
    }

    static RemoveLabelServiceDeclaration(afterWhereAndFuther) {
        // console.log(afterWhereAndFuther);
        var match = afterWhereAndFuther.match(/SERVICE\Wwikibase:label\W[^}]+\}/im);
        var before = afterWhereAndFuther.slice(0, match.index);
        var after = afterWhereAndFuther.slice(match.index + match[0].length);
        return before + after;
    }

    static InjectShortNames(inputString, marklessVariables) {
        // var marklessVariables = markedVariables
        //     .map(v => v.slice(1))
        //     ;
        var shortener = minify.GenerateShortener(marklessVariables); // delete question mark "?"

        var varMatch = undefined;
        var cuttedInputString = inputString;
        var result = [];
        while ((varMatch = cuttedInputString.match(/\?(\w+)/im)) !== null) {
            var prevString = cuttedInputString.slice(0, varMatch.index);
            var fullVar = cuttedInputString
                .slice(varMatch.index, varMatch.index + varMatch[0].length)
                .trim()
                .slice(1) // delete question mark "?"
                ;
            var shortVar = shortener(fullVar);
            result.push([prevString, AddMark(shortVar)]);
            cuttedInputString = cuttedInputString.slice(varMatch.index + varMatch[0].length);
        }
        result.push([cuttedInputString, ""]); //заключительный огрызок оставшийся после последней переменной
        return result
            .map(pair => pair[0] + " " + pair[1])
            .join("")
            ;
    }
}

module.exports = {
    QueryParser,
};