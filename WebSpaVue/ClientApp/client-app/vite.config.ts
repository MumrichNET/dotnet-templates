import {defineConfig} from "vite";
import vue from "@vitejs/plugin-vue";
import WindiCSS from "vite-plugin-windicss";

const protocol = process.env.HMR_PROTOCOL ?? "ws";
const port = Number(process.env.HMR_PORT) ?? 3000;

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [vue(), WindiCSS()],
  server: {
    port: 3000,
    strictPort: true,
    hmr: {
      protocol,
      port,
    },
  },
});
