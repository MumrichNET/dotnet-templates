<script setup lang="ts">
// This starter template is using Vue 3 <script setup> SFCs
// Check out https://v3.vuejs.org/api/sfc-script-setup.html#sfc-script-setup
import HelloWorld from "./components/HelloWorld.vue";</script>

<template>
  <div class="h-full w-full flex flex-col">
    <div>
      <img alt="Vue logo" src="./assets/logo.png" />
      <HelloWorld msg="Hello Vue 3 + TypeScript + Vite" />
    </div>
    <div class="flex-1 bg-gray-300">
      <iframe class="w-full h-full" src="/admin"></iframe>
    </div>
  </div>
</template>

<style>
html, body, #app {
  @apply h-full;
}
</style>
