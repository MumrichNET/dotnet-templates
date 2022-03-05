<template>
  <component
    v-for="(widget, index) in widgets"
    :key="generateKey(index)"
    :is="widget.component"
    v-bind="widget.props"
  />
</template>

<script lang="ts" setup>
import { computed } from 'vue';
import { useRoute } from 'vue-router';

const route = useRoute();
const widgets = computed(() => route.meta.widgets);

function generateKey(index: number) {
  return `${route.fullPath}-${index}`
}
</script>