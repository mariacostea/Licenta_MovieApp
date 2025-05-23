import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
  plugins: [react()],

  /* 🔄  Proxy spre backend (localhost:5000) –
         Browserul trimite cereri doar la /api,
         Vite le redirecţionează, deci CORS nu mai apare   */
  server: {
    proxy: {
      '/api': {
        target: 'http://localhost:5000',
        changeOrigin: false,   // origin rămâne http://localhost:5173
        secure: false
      }
    }
  }
});
