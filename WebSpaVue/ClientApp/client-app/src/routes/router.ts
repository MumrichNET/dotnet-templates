import { createRouter, createWebHashHistory } from "vue-router";
import HomeVue from "./Home.vue";
import AboutVue from "./About.vue";

const router = createRouter({
  history: createWebHashHistory(),
  routes: [
    {
      path: "/",
      component: HomeVue,
    },
    {
      path: "/about",
      component: AboutVue,
    },
  ],
});

export default router;
