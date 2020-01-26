
import { generate } from 'escodegen';
import convertToAst from './convert-to-ast';
import jsxVisitor from './jsx-visitor';

global.transpile = function (code) {
	try {
		const ast = convertToAst(fromBase64(code));

		const transpiledAst = jsxVisitor(ast);

		const result = generate(transpiledAst);

		return {
			status: true,
			result
		}
	} catch (error) {
		return {
			status: false,
			result: '',
			error: error.toString()
		}
	}
};