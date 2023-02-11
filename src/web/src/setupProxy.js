/* eslint-disable no-undef */
import { createProxyMiddleware } from 'http-proxy-middleware';

export default function(app) {
    if(process.env.API_PROXY_URL) {
        app.use(
            '/api',
            createProxyMiddleware({
                target: process.env.API_PROXY_URL,
                changeOrigin: true,
                pathRewrite: {
                    '^/api': '/'
                },
                ws: true
            })
        );
    }
}