<template>
  <nav class="flex flex-row shadow justify-around">
    <router-link v-for="route in routes" :key="route.path" :to="route.path">{{ route.meta.title }}</router-link>
  </nav>
</template>

<script lang="ts" setup>
import { computed } from "vue";
import { useRouter } from "vue-router";

const router = useRouter();
const routes = computed(() => router.getRoutes());
</script>
