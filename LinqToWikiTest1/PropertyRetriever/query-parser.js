const minify = require("./minify");
const { AddLabel, AddMark, IsNotLabelVariable, RemoveQuestionMark } = require("./util");


function RemoveDublicates(item, index, array) {
    return array.indexOf(item) === index;
}

function AddRdfsLabelClause(variable) {
    return `${AddMark(variable)} rdfs:label ${AddMark(AddLabel(variable))}. `;
    //same in functionnal programming style (with curring)
    // return `${AddMark(variable)} rdfs:label ${[variable].map(AddMark).map(AddLabel).join()}`;
}


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
        var afterWhereAndFuther = body.slice(firstParenthesisAfterWhereAnchor + 1);
        afterWhereAndFuther = this.InjectOptionalRdfsNames(afterWhereAndFuther);
        afterWhereAndFuther = this.ReplaceLabelServiceDeclaration(afterWhereAndFuther, variables, shortener);

        var composition = `${beforeSelectedIncluded} ${outVariables} where {\n${afterWhereAndFuther}`
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

    static ReplaceLabelServiceDeclaration(afterWhereAndFuther, variables, shortener) {
        var match = afterWhereAndFuther.match(/SERVICE\Wwikibase:label\W[^}]+\}/im);
        var before = afterWhereAndFuther.slice(0, match.index);
        var after = afterWhereAndFuther.slice(match.index + match[0].length);
        var rdfsLabelClauses = variables
            .map(shortener)
            .map(AddRdfsLabelClause)
            .join("\n");
        var labelServiceDeclaration = `SERVICE wikibase:label {bd:serviceParam wikibase:language "en". ${rdfsLabelClauses}}`;
        return `${before} ${labelServiceDeclaration} ${after}`;
    }

    static InjectShortNames(inputString, marklessVariables) {
        var shortener = minify.GenerateShortener(marklessVariables);

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