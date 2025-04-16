module.exports = () => {
	return {
		visitor: {
			AssignmentExpression(path) {
				if(path.node.operator !== '=') return;
				if(path.node.left.type !== 'MemberExpression' || path.node.right.type !== 'FunctionExpression') return;

				const pname = path.node.left.property.name;
				switch(pname) {
					case 'fromObject': case 'getTypeUrl': case 'toJSON':
						path.remove()
				}
			}
		}
	}
}