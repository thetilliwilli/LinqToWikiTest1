var bindingPrefix = "?";

function RemoveDublicates(item, index, array){
    return array.indexOf(item) === index;
}

class QueryParser
{
    static Parse(query) {

        var trimmed = query
            .trim()
            .toLowerCase()
            ;

        if(!trimmed.includes("select") || !trimmed.includes("where"))
            throw new Error("Incorrect query - cant find mandatory SELECT and WHERE clauses");
        var anchors = {
            justAfterSelect: trimmed.indexOf("select") + "select".length,
            justBeforeWhere: trimmed.indexOf("where"),
        };
        var variablesString = trimmed.slice(anchors.justAfterSelect, anchors.justBeforeWhere);
        var variables = 
            (() => variablesString.trim() === "*"
                ? trimmed.slice(anchors.justBeforeWhere + "where".length)
                    .match(/\?[a-zA-z0-9]+/ig)
                    .filter(RemoveDublicates)
                : variablesString
                    .trim()
                    .split(bindingPrefix)
                    .slice(1)
                    .map(s => s.trim())
                    .filter(s=>s.slice(-"label".length) !== "label")
            )();
        var result = new QueryDescription();
        result.variables = variablesString
            .trim()
            .split(bindingPrefix)
            .slice(1)
            .map(s => s.trim())
            .filter(s=>s.slice(-"label".length) !== "label")
            ;
        result.original = query;
        return result;
    }
}

class QueryDescription
{
    // variables;
    // original;
}

module.exports = {
    QueryParser,
};