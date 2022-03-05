<template>
  <div class>
    <q-splitter v-model="splitterModel" horizontal>
      <template v-slot:before>
        <q-tabs v-model="tab" vertical class="text-teal">
          <q-tab name="markdown" icon="edit_note" label="Markdown" />
        </q-tabs>
      </template>
      <template v-slot:after>
        <q-tab-panels
          v-model="tab"
          animated
          swipeable
          vertical
          transition-prev="jump-up"
          transition-next="jump-up"
        >
          <q-tab-panel name="markdown" class="bg-grey-2">
            <q-input type="textarea" counter v-model="modelValue" />
          </q-tab-panel>
        </q-tab-panels>
      </template>
    </q-splitter>
  </div>
</template>

<script lang="ts" setup>
import { ref } from 'vue';

const tab = ref('markdown');
const splitterModel = ref(20);
const modelValue = ref('# Hello Markdown 😍\nThis *is* a **text**!')
</script>
