const commonjs = require('@rollup/plugin-commonjs');
const nodeResolve = require('@rollup/plugin-node-resolve');
const json = require('@rollup/plugin-json');
const babel = require('rollup-plugin-babel');
const minify = require('rollup-plugin-babel-minify');
const replace = require('@rollup/plugin-replace');
const typescript = require('@rollup/plugin-typescript');

const { PRODUCTION } = process.env

module.exports = {
    input: 'src/index.ts',
    output: {
      file: '../../dist/unity-renderer.js',
      format: 'esm'
    },
    external: [ 'react' ],
    plugins: [
      typescript(),
      babel(),
      replace({
        'process.env.NODE_ENV': JSON.stringify(
           PRODUCTION ? 'production' : 'development'
         )
      }),
      commonjs({
        ignoreGlobal: false,
        include: ['node_modules/**'],
      }),
      nodeResolve({
        jsnext: true,
        preferBuiltins: false
      }),
      json(),  
      minify({
        comments: false
      }),
    ]
  };