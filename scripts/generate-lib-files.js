const fs = require('fs');
const path = require('path');

const escapeString = (str) => {
    return str
        .replace(/\\/g, '\\\\')
        .replace(/\n/g, '\\n')
        .replace(/\t/g, '\\t')
        .replace(/\r/g, '\\r')
        .replace(/\f/g, '\\f')
        .replace(/\"/g, '\\"');
}

const template = fs.readFileSync(path.join(__dirname, 'lib-files-template.txt')).toString();
const babelTransform = fs.readFileSync(path.join(__dirname, '../dist/babel-transform.js')).toString();
const react = fs.readFileSync(path.join(__dirname, '../dist/react.js')).toString();
const unityRenderer = fs.readFileSync(path.join(__dirname, '../dist/unity-renderer.js')).toString();

const replacer = (replacement) => () => replacement;

const result = template
    .replace('##babel_transform##', replacer(escapeString(babelTransform)))
    .replace('##react##', replacer(escapeString(react)))
    .replace('##unity_renderer##', replacer(escapeString(unityRenderer)));

fs.writeFileSync(path.join(__dirname, '../packages/uielements.react/Runtime/JsRuntime/JSLibraries.cs'), result);
