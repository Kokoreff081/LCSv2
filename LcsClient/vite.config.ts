import {defineConfig} from 'vite';
import vue from '@vitejs/plugin-vue';
import path from 'path';
import basicSsl from '@vitejs/plugin-basic-ssl';
import LCHubPlugin from './src/LCHubPlugin';

// https://vitejs.dev/config/
export default defineConfig({
    mode: 'development',
    plugins: [vue(), basicSsl(), LCHubPlugin],
    base: '',
    resolve: {
        alias: {
            '@': path.resolve(__dirname, './src'),
            '@store': path.resolve(__dirname, './src/store'),
            '@components': path.resolve(__dirname, './src/components'),
            '@modules': path.resolve(__dirname, './src/modules'),
            '@pages': path.resolve(__dirname, './src/pages'),
            '@lcHubPlugin': path.resolve(__dirname, './src/LcHubPlugin.ts')
        }
    }
});
