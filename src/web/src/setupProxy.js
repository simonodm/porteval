const { createProxyMiddleware } = require('http-proxy-middleware');

module.exports = function(app) {
    if(process.env.API_PROXY_URL) {
        app.use(
            '/api',
            createProxyMiddleware({
                target: process.env.API_PROXY_URL,
                changeOrigin: true,
                pathRewrite: {
                    '^/api': '/'
                }
            })
        );
    }
}