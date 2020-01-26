// Copied from https://github.com/babel/babel/blob/eac4c5bc17133c2857f2c94c1a6a8643e3b547a7/packages/babel-types/src/utils/react/cleanJSXElementLiteralChild.js
// License: https://github.com/babel/babel/blob/master/LICENSE

export default function cleanJSXElementLiteralChild(child, args) {
    const lines = child.value.split(/\r\n|\n|\r/);
  
    let lastNonEmptyLine = 0;
  
    for (let i = 0; i < lines.length; i++) {
      if (lines[i].match(/[^ \t]/)) {
        lastNonEmptyLine = i;
      }
    }
  
    let str = "";
  
    for (let i = 0; i < lines.length; i++) {
      const line = lines[i];
  
      const isFirstLine = i === 0;
      const isLastLine = i === lines.length - 1;
      const isLastNonEmptyLine = i === lastNonEmptyLine;
  
      // replace rendered whitespace tabs with spaces
      let trimmedLine = line.replace(/\t/g, " ");
  
      // trim whitespace touching a newline
      if (!isFirstLine) {
        trimmedLine = trimmedLine.replace(/^[ ]+/, "");
      }
  
      // trim whitespace touching an endline
      if (!isLastLine) {
        trimmedLine = trimmedLine.replace(/[ ]+$/, "");
      }
  
      if (trimmedLine) {
        if (!isLastNonEmptyLine) {
          trimmedLine += " ";
        }
  
        str += trimmedLine;
      }
    }
  
    if (str) args.push({
        type: 'Literal',
        value: str,
        raw: `'${str}'`
    });
  }