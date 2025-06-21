import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
    plugins: [react()],
    preview: {
      host: '0.0.0.0', // ⬅️ acest rând lipsește la tine
      port: 4173,
      allowedHosts: ['licenta-frontend-r2gn.onrender.com']
    },
    server: {
      proxy: {
        '/api': {
          target: 'https://licenta-backend-nf1m.onrender.com',
          changeOrigin: true,
          secure: false
        }
      }
    }
  });

