import axios from 'axios';

const controller = new AbortController();

export const HTTP = axios.create({
    baseURL: 'https://localhost:7291',
    signal: controller.signal,
});