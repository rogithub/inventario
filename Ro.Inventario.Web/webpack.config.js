const path = require('path');

module.exports = {
    entry: {
		ventas: './scripts/pages/ventas.ts'
    },
    mode: 'production',
    optimization: {
		minimize: false,
		splitChunks: {
			chunks: 'all',
			minSize: 0,
			name: 'shared'
		}
    },
    module: {
		rules: [
		{
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
