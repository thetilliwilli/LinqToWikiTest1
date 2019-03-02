async function read(stream) {
    let buffer = Buffer.alloc(0);
    for await (const chunk of stream) {
        buffer = Buffer.concat([buffer, chunk]);
    }
    return buffer.toString('utf8');
}


function read2() {
    return new Promise((RS, RJ) => {
        // console.log("read2");
        var text = "";
        process.stdin.setEncoding("utf8");

        process.stdin.on("readable", () => {
            // console.log("on-readable");
            var chunk = process.stdin.read();
            if (chunk !== null)
                text += chunk;
        });

        process.stdin.on("end", () => {
            // console.log("on-end");
            var chunk = process.stdin.read();
            // console.log("chunk", chunk);
            if (chunk !== null)
                text += chunk;
            // console.log(text);
            RS(text);
        });

        process.stdin.on("error", RJ);
    });
}

module.exports = {
    read: read2,
};