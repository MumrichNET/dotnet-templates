<template>
  <q-splitter horizontal v-model="editVsPreviewSplitterModel">
    <template v-slot:before>
      <q-btn @click="sendMessageToChild">Send</q-btn>
    </template>
    <template v-slot:after>
      <iframe ref="iframe" id="client-app" :src="iFrameSrc" width="100%" height="500px" />
    </template>
  </q-splitter>
</template>

<script lang="ts" setup>
import { computed, ref } from 'vue';

const editVsPreviewSplitterModel = ref(50);
const iFrameSrc = computed(() => window.location.origin);
const iframe = ref<HTMLIFrameElement | null>(null);

function sendMessageToChild() {
  if (iframe.value) {
    iframe.value.contentDocument?.dispatchEvent(new CustomEvent('setWidgets', {
      detail: {
        routes: [
          {
            path: '/',
            meta: {
              title: 'Test',
              widgets: [
                {
                  component: 'w-markdown',
                  props: {
                    markdown: '# Test'
                  }
                }
              ]
            }
          }
        ]
      }
    }))
  }
}
</script>
