const path = require('path');

module.exports = {
    entry: {
	app: './scripts/pages/app.ts'
    },
    mode: 'production',
    optimization: {
	minimize: false,
	splitChunks: {
	    cunks: 'all',
	    minSize: 0,
	    name: 'shared'
	}
    },
    module: {
	rules: [{
	    test: /\.tsx?$/,
	    use: 'ts-loader',
	    exclude: '/node_modules/'
	}]
    },
    resolve: {
	extensions: ['.tsx','.ts','.js']
    },
    output: {
	filename: '[name].js',
	path: path.resolve(__dirname, 'wwwroot/js')
    },
    externals: {
	'ko': 'ko'
    }
};
