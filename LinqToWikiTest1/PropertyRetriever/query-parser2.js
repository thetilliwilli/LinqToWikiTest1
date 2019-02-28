var bindingPrefix = "?";
var labelAppendix = "Label";
var languageServiceTag = "en";

function RemoveDublicates(item, index, array){
    return array.indexOf(item) === index;
}

function AddLabel(variable){
    return variable + labelAppendix;
}

function AddLabelFilterClause(variable){
    return `FILTER(LANG(?${variable}Label)="en")`
}

function AddVariableQuestionPrefix(variable){
    return "?"+variable;
}

function Sort(a,b){return a.localeCompare(b);}

class QueryParser
{
    static Parse(query) {

        var trimmed = query.toLowerCase();

        if(!trimmed.includes("select") || !trimmed.includes("where"))
            throw new Error("Incorrect query - cant find mandatory SELECT and WHERE clauses");

        var anchors = {
            justAfterSelect: trimmed.indexOf("select") + "select".length,
            justBeforeWhere: trimmed.indexOf("where"),
            firstParenthesisAfterWhere: trimmed.indexOf("{", trimmed.indexOf("where") + "where".length),
        };

        var variables = query
            .match(/\?[a-zA-Z0-9_]+/img)
            .filter(RemoveDublicates)
            .filter(v=>v.slice(-"Label".length) !== "Label")
            .map(v=>v.slice(1))
            ;

        return { variables, body:query.trim(), anchors, };
    }

    static InjectLabels(query){
        var body = query.body
            
        var markedVariables = query.variables
            .sort(Sort)
            .map(AddVariableQuestionPrefix)
            ;

        var beforeSelectedIncluded = body.slice(0, query.anchors.justAfterSelect);
        var outVariables = [...markedVariables, ...markedVariables.map(AddLabel)].join(" ").trim();
        var languageFilters = query.variables.map(AddLabelFilterClause).join("\n");
        var afterWhereAndFuther = body.slice(query.anchors.firstParenthesisAfterWhere + 1);
            afterWhereAndFuther = this.InjectOptionalRdfsNames(afterWhereAndFuther);

        var composition = `${beforeSelectedIncluded} ${outVariables} where { \n${languageFilters}\n${afterWhereAndFuther}`
            .replace(/\n/img, " ")
            .replace(/  /img, " ")
            ;
        return composition;
    }


    static InjectOptionalRdfsNames(afterWhereAndFuther){
        var matchesOfOptions = afterWhereAndFuther.match(/OPTIONAL\W[^}]+\}/img);
        var firstMatchIndex = afterWhereAndFuther.indexOf(matchesOfOptions[0]);
        var lastMatchIndex =  afterWhereAndFuther.lastIndexOf(matchesOfOptions[matchesOfOptions.length-1]);
        var lastMatchLength = matchesOfOptions[matchesOfOptions.length-1].length;

        var optionalList = matchesOfOptions;
        //OPTIONAL { ?game wdt:P400 ?platform. }
        //OPTIONAL { ?game wdt:P400 ?platform; rdfs:label ?platformLabel. }
        var optionalsWithRdfsName = optionalList
            .map(option => {
                var matches = option.match(/(\?\w+)/img);
                if(!matches) throw new Error("no matches in OPTION - its strange");
                var lastMatch = matches.pop();
                var indexOfLastMatch = option.lastIndexOf(lastMatch);
                return `${option.slice(0, indexOfLastMatch)} ${lastMatch}; rdfs:label ${lastMatch}${labelAppendix}. }`;
            })
            .join("\n")
            ;

        return `${afterWhereAndFuther.slice(0, firstMatchIndex)} ${optionalsWithRdfsName} ${afterWhereAndFuther.slice(lastMatchIndex+lastMatchLength)}`;
    }
}

module.exports = {
    QueryParser,
};