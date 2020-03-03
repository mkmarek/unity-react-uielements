import transformer from './TransformJSXToReactBabelPlugin';
import * as walk from 'acorn-walk';
import { extend } from 'acorn-jsx-walk';
import babel from './babel';

extend(walk.base);

function sanitizeVisitor(visitor, findAncestor) {
	const keys = Object.keys(visitor);
	const newVisitor = {};

	for (let key of keys) {
		newVisitor[key] = (node, state, c) => {
			const path = {
				node,
				get(name) {
					return {
						parent: node,
						node: node[name]
					}
				},
				replaceWith(replacement) {
					const ancestor = findAncestor(node);

					if (!ancestor) return;

					if (ancestor.type === 'CallExpression') {
						const index = ancestor.arguments.indexOf(node);

						if (index < 0) {
							throw new Error(`No argument found to replace in CallExpression`)
						}

						ancestor.arguments[index] = replacement;
					} else if (ancestor.type === 'JSXElement') {
						const index = ancestor.children.indexOf(node);

						if (index < 0) {
							throw new Error(`No argument found to replace in CallExpression`)
						}

						ancestor.children[index] = replacement;
					} else if (ancestor.type === 'ArrowFunctionExpression') {
						ancestor.body = replacement;
					} else if (ancestor.type === 'VariableDeclarator') {
						ancestor.init = replacement;
					} else if (ancestor.type === 'ReturnStatement') {
						ancestor.argument = replacement;
					} else if (ancestor.type === 'LogicalExpression') {
						if (ancestor.left.type === 'JSXElement') {
							ancestor.left = replacement;
						} else {
							ancestor.right = replacement;
						}
					} else {
						console.log(JSON.stringify(ancestor));
						throw new Error(`Unknown ${ancestor.type} element in replaceWith`)
					}
				}
			}

			const options =  {
				opts: {},
				get: (key) => state[key],
				set: (key, value) => state[key] = value
			};

			if (visitor[key].enter) {
				visitor[key].enter(path, options);
			}

			walk.base[key](node,state,c);

			if (visitor[key].exit) {
				visitor[key].exit(path, options);
			}
		}
	}

	return newVisitor;
}

export default function run(ast) {
    const copy = JSON.parse(JSON.stringify(ast));
    const findAncestor = (node) => {
        let ancestor = null;
        walk.fullAncestor(copy, (test, ancestors) => {
            if (node === test) {
                ancestor = ancestors[ancestors.length - 2];
            }
        });

        return ancestor;
    }

    const visitor = sanitizeVisitor(transformer(babel).visitor, findAncestor);

    walk.recursive(copy, { ancestors: [] }, visitor);

    return copy;
}