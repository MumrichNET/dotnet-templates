<template>
  <q-splitter v-model="splitterModel" style="height: 100%;">
    <template v-slot:before>
      <q-tree :nodes="simple" node-key="label" v-model:selected="selected" default-expand-all />
    </template>
    <template v-slot:after>
      <h6>{{ selected }}</h6>
    </template>
  </q-splitter>
</template>

<script lang="ts" setup>
import { ref } from 'vue';

const selected = ref(null);
const splitterModel = ref(50)
const simple = ref([
  {
    label: 'Satisfied customers (with avatar)',
    avatar: 'https://cdn.quasar.dev/img/boy-avatar.png',
    children: [
      {
        label: 'Good food (with icon)',
        icon: 'restaurant_menu',
        children: [
          { label: 'Quality ingredients' },
          { label: 'Good recipe' }
        ]
      },
      {
        label: 'Good service (disabled node with icon)',
        icon: 'room_service',
        disabled: true,
        children: [
          { label: 'Prompt attention' },
          { label: 'Professional waiter' }
        ]
      },
      {
        label: 'Pleasant surroundings (with icon)',
        icon: 'photo',
        children: [
          {
            label: 'Happy atmosphere (with image)',
            img: 'https://cdn.quasar.dev/img/logo_calendar_128px.png'
          },
          { label: 'Good table presentation' },
          { label: 'Pleasing decor' }
        ]
      }
    ]
  }
])
</script>
