import { fileURLToPath, URL } from 'node:url'

import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import vueDevTools from 'vite-plugin-vue-devtools'

// https://vite.dev/config/
export default defineConfig({
  plugins: [
    vue(),
    vueDevTools(),
  ],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url)),
    },
  },
  // Явно слушаем IPv4: иначе на Windows Vite встаёт только на ::1,
  // а браузер при системном прокси (VPN) ходит на 127.0.0.1 и не достучится.
  server: {
    host: '127.0.0.1',
    port: 5173,
  },
})
