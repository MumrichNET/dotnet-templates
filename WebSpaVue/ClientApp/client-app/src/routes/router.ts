import { Plugin } from "vue";
import { createRouter, createWebHashHistory } from "vue-router";
import { useStore } from "../pinia/pinia";

const router: Plugin = {
  install(app, _options) {
    const store = useStore();

    const router = createRouter({
      history: createWebHashHistory(),
      routes: store.$state.routes,
    });

    app.use(router);
  },
};

export default router;
