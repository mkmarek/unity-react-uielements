const commonjs = require('@rollup/plugin-commonjs');
const nodeResolve = require('@rollup/plugin-node-resolve');
const json = require('@rollup/plugin-json');
const babel = require('rollup-plugin-babel');
const minify = require('rollup-plugin-babel-minify');
const replace = require('@rollup/plugin-replace');

const { PRODUCTION } = process.env

module.exports = {
    input: 'src/index.js',
    output: {
      file: '../../dist/react.js',
      format: 'esm'
    },
    plugins: [
      replace({
        'process.env.NODE_ENV': JSON.stringify(
           PRODUCTION ? 'production' : 'development'
         )
      }),
      commonjs({
        ignoreGlobal: false,
        include: ['node_modules/**'],
        namedExports: {
          'node_modules/react/index.js': ['Children','createRef','Component','PureComponent','createContext','forwardRef','lazy','memo','useCallback','useContext','useEffect','useImperativeHandle','useDebugValue','useLayoutEffect','useMemo','useReducer','useRef','useState','Fragment','Profiler','StrictMode','Suspense','createElement','cloneElement','createFactory','isValidElement','version']
        },
      }),
      nodeResolve({
        jsnext: true
      }),
      json(),
      babel(),  
      minify({
        comments: false
      }),
    ]
  };