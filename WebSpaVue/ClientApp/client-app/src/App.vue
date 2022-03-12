<template>
  <NavVue />
  <router-view></router-view>
</template>

<script lang="ts" setup>
import NavVue from "./components/Nav.vue";
import { RouteRecordRaw } from "vue-router";
import { onMounted } from "vue";
import { useStore } from "./pinia/pinia";

const store = useStore();

function handleSetWidgets(e: any) {
  const payload = e.detail as RouteRecordRaw[];

  store.setRoutes(payload);
}

onMounted(() => {
  window.document.addEventListener("setWidgets", handleSetWidgets)
})
</script>
