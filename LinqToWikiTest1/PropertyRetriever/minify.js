function* f1(){for(var i="a".charCodeAt(0);i<="z".charCodeAt(0);i++) yield String.fromCharCode(i);return;}
function* f2(){for(var i="A".charCodeAt(0);i<="Z".charCodeAt(0);i++) yield String.fromCharCode(i);return;}
var shorteningDic = [...f1(),...f2()];

//inputString
inputString
    .trim()
    .slice(1)
    .split(" ?")
    .map((name,i)=>({name:name,short:shorteningDic[i]}))
    ;