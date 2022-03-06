import { createRouter, createWebHashHistory, RouteRecordRaw } from "vue-router";
import HomeVue from "./Home.vue";
import AboutVue from "./About.vue";

export const routes: RouteRecordRaw[] = [
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
        {
          component: "w-markdown",
          props: {
            markdown: "This *is* a **text**!",
          },
        },
      ],
    },
  },
  {
    path: "/test",
    component: { template: "<h1>Test</h1>" },
    meta: {
      title: "Test",
      widgets: [
        {
          component: "w-markdown",
          props: {
            markdown: "# Test",
          },
        },
      ],
    },
    children: [
      {
        path: "test-child",
        component: { template: "<h2>Test-Child</h2>" },
        meta: {
          title: "Test-Child",
          widgets: [
            {
              component: "w-markdown",
              props: {
                markdown: "# Test-Child",
              },
            },
          ],
        },
      },
    ],
  },
  {
    path: "/about",
    component: AboutVue,
    meta: {
      title: "About",
    },
  },
];

export const router = createRouter({
  history: createWebHashHistory(),
  routes,
});

export default router;
