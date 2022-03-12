import { defineStore } from "pinia";
import { RouteRecordRaw } from "vue-router";
import AboutVue from "../routes/About.vue";
import DynamicPageRendererVue from "../components/DynamicPageRenderer.vue";

const routes: RouteRecordRaw[] = [
  {
    path: "/",
    component: DynamicPageRendererVue,
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
    component: DynamicPageRendererVue,
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
        component: DynamicPageRendererVue,
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

export const useStore = defineStore("clientRoutes", {
  state: () => {
    return {
      routes,
    };
  },
});
