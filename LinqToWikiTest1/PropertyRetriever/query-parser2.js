const minify = require("./minify");

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

class QueryParser {
    static Parse(query) {

        var trimmed = query.toLowerCase();

        if (!trimmed.includes("select") || !trimmed.includes("where"))
            throw new Error("Incorrect query - cant find mandatory SELECT and WHERE clauses");

        var anchors = {
            justAfterSelect: trimmed.indexOf("select") + "select".length,
            justBeforeWhere: trimmed.indexOf("where"),
            firstParenthesisAfterWhere: trimmed.indexOf("{", trimmed.indexOf("where") + "where".length),
        };

        var variables = query
            .match(/\?[a-zA-Z0-9_]+/img)
            .filter(RemoveDublicates)
            .filter(v => v.slice(-"Label".length) !== "Label")
            .map(v => v.slice(1))
            ;

        return { variables, body: query.trim(), anchors, };
    }


   

    static InjectLabels(query) {

        var markedVariables = query.variables
            .sort(Sort)
            .map(AddMark)
            ;

        var shortener = minify.GenerateShortener(query.variables);
        var body = this.InjectShortNames(query.body, markedVariables);
        var trimmed = body.toLowerCase();


        var justAfterSelectAnchor = trimmed.indexOf("select") + "select".length;
        var firstParenthesisAfterWhereAnchor = trimmed.indexOf("{", trimmed.indexOf("where") + "where".length);


        var beforeSelectedIncluded = body.slice(0, justAfterSelectAnchor);
        var outVariables = [
            ...query.variables.map(shortener).map(AddMark), 
            ...query.variables.map(shortener).map(AddMark).map(AddLabel), 
        ].join(" ").trim();
        var languageFilters = query.variables.map(shortener).map(AddMark).map(AddLabel).map(AddLabelFilterClause).join("\n");
        var afterWhereAndFuther = body.slice(firstParenthesisAfterWhereAnchor + 1);
        afterWhereAndFuther = this.InjectOptionalRdfsNames(afterWhereAndFuther);
        afterWhereAndFuther = this.RemoveLabelServiceDeclaration(afterWhereAndFuther);

        var composition = `${beforeSelectedIncluded} ${outVariables} where { \n${languageFilters}\n${afterWhereAndFuther}`
            .replace(/\n/img, " ")
            .replace(/  /img, " ")
            ;

        return composition;
    }


    /** OPTIONAL { ?game wdt:P400 ?platform. } -> OPTIONAL { ?game wdt:P400 ?platform. ?platform rdfs:label ?platformLabel. } */
    static InjectOptionalRdfsNames(afterWhereAndFuther) {
        var matchesOfOptions = afterWhereAndFuther.match(/OPTIONAL\W[^}]+\}/img);
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
        var match = afterWhereAndFuther.match(/SERVICE\Wwikibase:label\W[^}]+\}/);
        var before = afterWhereAndFuther.slice(0, match.index);
        var after = afterWhereAndFuther.slice(match.index + match[0].length);
        return before + after;
    }

    static InjectShortNames(inputString, markedVariables) {
        var marklessVariables = markedVariables
            .map(v => v.slice(1))
            ;
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