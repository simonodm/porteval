/* eslint-disable no-undef */
// eslint-disable-next-line @typescript-eslint/no-var-requires
const { createProxyMiddleware } = require('http-proxy-middleware');

// This file is kept as JS instead of TS, as CRA bundling seems to have problems with proxy setup in TS

// This function sets up a reverse proxy when running the application using `react-scripts start`.
// For other (static) deployments, this proxy needs to be set up in web server configuration.
module.exports = function(app) {
    if(process.env.PORTEVAL_API_URL) {
        app.use(
            '/api',
            createProxyMiddleware({
                target: process.env.PORTEVAL_API_URL,
                changeOrigin: true,
                pathRewrite: {
                    '^/api': '/'
                },
                ws: true
            })
        );
    }
}