import { createRouter, createWebHashHistory } from "vue-router";
import HomeVue from "./Home.vue";
import AboutVue from "./About.vue";

const router = createRouter({
  history: createWebHashHistory(),
  routes: [
    {
      path: "/",
      component: HomeVue,
      meta: {
        title: "Home",
        widgets: [
          {
            component: "w-markdown",
            props: {
              markdown: "# Hello Markdown 😍",
            },
          },
        ],
      },
    },
    {
      path: "/about",
      component: AboutVue,
      meta: {
        title: "About",
      },
    },
  ],
});

export default router;
