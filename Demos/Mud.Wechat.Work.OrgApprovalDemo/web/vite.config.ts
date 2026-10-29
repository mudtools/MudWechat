import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

// 产物直接落 ../wwwroot 并**提交入库**：fresh clone 后 `dotnet run` 即可看到页面，
// 全量构建与门禁都不需要 Node（所以 npm 绝不挂进 MSBuild，见 Demos/README.md §6.9）。
export default defineConfig({
  base: '/',
  plugins: [vue()],
  build: {
    outDir: '../wwwroot',
    emptyOutDir: true,
    assetsDir: 'assets',

    // 去掉 contenthash：否则每次构建产生无意义 diff，「两次 build 后 git status 干净」不成立。
    rollupOptions: {
      output: {
        entryFileNames: 'assets/app.js',
        chunkFileNames: 'assets/[name].js',
        assetFileNames: 'assets/[name][extname]',
      },
    },
  },
  server: {
    port: 5173,
    // 开发模式：前后端分离，代理到 Demo 的 http 端口（生产形态同源，无需 CORS）。
    proxy: {
      '/api': 'http://localhost:56571',
      '/healthz': 'http://localhost:56571',
    },
  },
})
