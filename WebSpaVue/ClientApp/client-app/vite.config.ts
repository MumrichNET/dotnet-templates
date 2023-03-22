import { defineConfig } from "vite";
import vue from "@vitejs/plugin-vue";
import WindiCSS from "vite-plugin-windicss";

const config = {
  plugins: [vue(), WindiCSS()],
  server: {
    host: true,
    port: 3000,
    strictPort: true
  },
};

console.log(`*** vite-config: ${JSON.stringify(config)}`);

// https://vitejs.dev/config/
export default defineConfig(config);
