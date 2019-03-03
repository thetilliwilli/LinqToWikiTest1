const inputGetter = require("./inputGetter");
const { QueryParser } = require("./query-parser");
const csExporter = require("./exportToCs")

inputGetter.getInputString()
    .then(inputString => {
        var parsedQuery = QueryParser.Parse(inputString);
        console.log("\n");
        console.log(parsedQuery.composition);

        console.log("\n");
        console.log("-".repeat(100));
        console.log("\n");

        var yyy = csExporter.GenerateCsClass("YYYY", parsedQuery.shortdic);
        console.log(yyy);
    })
    .catch(console.error)
    ;