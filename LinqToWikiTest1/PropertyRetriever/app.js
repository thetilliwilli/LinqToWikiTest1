const inputGetter = require("./inputGetter");
const { QueryParser } = require("./query-parser");
const csExporter = require("./exportToCs")

inputGetter.getInputString()
    .then(inputString => {
        var xxx = QueryParser.Parse(inputString);
        console.log("\n");
        console.log(xxx.composition);

        console.log("\n");
        console.log("-".repeat(100));
        console.log("\n");

        var yyy = csExporter.GenerateCsClass("YYYY", xxx.variables);
        console.log(yyy);
    })
    .catch(console.error)
    ;