<template>
  <NavVue />
  <router-view></router-view>
  <button @click="setRoutes">setRoutes</button>
</template>

<script lang="ts" setup>
import NavVue from "./components/Nav.vue";
import { RouteRecordRaw } from "vue-router";
import { onMounted } from "vue";
import { useStore } from "./pinia/pinia";
import DynamicPageRendererVue from "./components/DynamicPageRenderer.vue";

const store = useStore();

function setRoutes() {
  store.setRoutes([
    {
      path: "/",
      component: DynamicPageRendererVue,
      meta: {
        title: "Test",
        widgets: [
          {
            component: "w-markdown",
            props: {
              markdown: "# Test"
            }
          }
        ]
      }
    }
  ])
}

function handleSetWidgets(e: any) {
  const payload = e.detail as RouteRecordRaw[];

  store.setRoutes(payload);
}

onMounted(() => {
  window.document.addEventListener("setWidgets", handleSetWidgets)
})
</script>
