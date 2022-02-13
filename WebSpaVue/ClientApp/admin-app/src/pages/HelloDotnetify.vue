<template>
  <div class="hello">
    <div v-if="state">
      <h3>{{ state.Greetings }}</h3>
      <p>Server time is: {{ state.ServerTime }}</p>
    </div>
  </div>
</template>

<script setup lang="ts">
import dotnetify from 'dotnetify/dist/dotnetify';
import { IDotnetifyVM } from 'dotnetify';
import { onMounted, onUnmounted, ref } from 'vue';

dotnetify.hubServerUrl = 'https://localhost:7189';

type HelloDotnetify = {
  Greetings: string;
  ServerTime: string;
};

const vm = ref<IDotnetifyVM | null>(null);
const state = ref<HelloDotnetify>();

onMounted(
  () =>
    (vm.value = dotnetify.connect('HelloDotnetify', {
      setState: (newState: HelloDotnetify) => {
        state.value = { ...state.value, ...newState };
      },
    }))
);
onUnmounted(() => vm.value?.$destroy());
</script>
