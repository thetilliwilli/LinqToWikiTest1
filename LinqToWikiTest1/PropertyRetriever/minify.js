
const { IsLabelVariable, originalLabelAppendix } = require("./util");

function* azGenerator() { for (var i = "a".charCodeAt(0); i <= "z".charCodeAt(0); i++) yield String.fromCharCode(i); return; }
function* AZGenerator() { for (var i = "A".charCodeAt(0); i <= "Z".charCodeAt(0); i++) yield String.fromCharCode(i); return; }
function* azIndexedGenerator() {
    for (var c = "a".charCodeAt(0); c <= "z".charCodeAt(0); c++)
        for (var i = 0; i <= 9; i++)
            yield String.fromCharCode(c) + i; //a0, a1, ... a9
    return;
}
function* AZIndexedGenerator() {
    for (var c = "A".charCodeAt(0); c <= "Z".charCodeAt(0); c++)
        for (var i = 0; i <= 9; i++)
            yield String.fromCharCode(c) + i; //a0, a1, ... a9
    return;
}

function GenerateShorteningDictionary(n) {
    var shorteningDic = [
        ...azGenerator(), //cardinality 26
        ...AZGenerator(), //cardinality 26
        ...azIndexedGenerator(), //cardinality 260
        ...AZIndexedGenerator(), //cardinality 260
    ]; //total cardinality 572
    if (shorteningDic.length < n)
        throw new Error("Too many cardinality required");
    return shorteningDic;
}

function GenerateShortener(names) {
    names = names.sort((a, b) => a.localeCompare(b));
    var shortDic = GenerateShorteningDictionary(names.length);
    function Shortener(name) {
        if (IsLabelVariable(name)) {
            var index = names.indexOf(name.slice(0, -originalLabelAppendix.length));
            if (index === -1) throw new Error("Out of range");
            return shortDic[index] + originalLabelAppendix;
        }
        else {
            var index = names.indexOf(name);
            if (index === -1) throw new Error("Out of range");
            return shortDic[index];
        }


    }
    Shortener.shortDic = names
        .map(name => ({ name: name, short: Shortener(name) }))
        ;
    return Shortener;
}

module.exports = {
    GenerateShortener,
};