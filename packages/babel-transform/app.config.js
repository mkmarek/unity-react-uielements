const commonjs = require('@rollup/plugin-commonjs');
const nodeResolve = require('@rollup/plugin-node-resolve');
const json = require('@rollup/plugin-json');
const babel = require('rollup-plugin-babel');
const minify = require('rollup-plugin-babel-minify');

module.exports = {
    input: 'src/index.js',
    output: {
      file: '../../dist/babel-transform.js',
      format: 'iife'
    },
    plugins: [
      minify({
        comments: false
      }),
      commonjs({
        ignoreGlobal: false,
        include: ['node_modules/**'],
      }),
      nodeResolve({
        jsnext: false
      }),
      json(),
      babel()
    ]
  };