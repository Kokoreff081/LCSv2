const path = require('path');
const { CleanWebpackPlugin } = require('clean-webpack-plugin');
const { VueLoaderPlugin } = require('vue-loader');
const HtmlWebpackPlugin = require('html-webpack-plugin');


module.exports = {
    mode: 'development',
    entry: {
        main: path.resolve(__dirname, './src/index.js'),
        commonHttp: path.resolve(__dirname, './src/global/commonHttpRequest.js'),
        lcHubPlugin: path.resolve(__dirname, './src/LCHubPlugin.js'),
    },
    output: {
        path: path.resolve(__dirname, './dist/'),
        filename: '[name].js',
    },
    devServer:{
        allowedHosts: 'all',
        https: true,
        host: '127.0.0.1',
        port: 7292
    },
    plugins: [
        new CleanWebpackPlugin(),
        new VueLoaderPlugin(),
        new HtmlWebpackPlugin({
            title: 'LightControlService',
            template: 'index.html'
        }),
    ],
    module: {
        rules: [
            // JavaScript
            {
                test: /\.js$/,
                exclude: /node_modules/,
                use: ['babel-loader'],
            },
            // изображения
            {
                test: /\.(?:ico|gif|png|jpg|jpeg)$/i,
                type: 'asset/resource',
            },
            // шрифты и SVG
            {
                test: /\.(woff(2)?|eot|ttf|otf|svg|)$/,
                type: 'asset/inline',
            },
            {
                test: /\.vue$/,
                loader: 'vue-loader',
            }
            ,
            {
                test: /\.css$/,
                loader: 'css-loader',
                options: {
                    esModule: false
                }
            },
            {
                test: /\.(json5?|ya?ml)$/, // target json, json5, yaml and yml files
                type: 'javascript/auto',
                // Use `Rule.include` to specify the files of locale messages to be pre-compiled
                include: [
                    path.resolve(__dirname, './src/locales'),
                ],
                loader: '@intlify/vue-i18n-loader'
            },
            // for i18n custom block
            {
                resourceQuery: /blockType=i18n/,
                type: 'javascript/auto',
                loader: '@intlify/vue-i18n-loader'
            }
        ],
    }
}