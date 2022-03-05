<template>
  <div v-html="purifiedHtml" />
</template>

<script lang="ts" setup>
import { computed, PropType } from "vue";
import { marked } from "marked";
import DOMPurify, { Config } from "dompurify";

const props = defineProps({
  markdown: {
    required: true,
    type: String,
  },
  sanitize: {
    required: false,
    type: Object as PropType<Config>,
    default: () => ({ ADD_ATTR: ["target"] }),
  },
});

const html = computed(() => marked.parse(props.markdown ?? "[invalid markdown]"));
const purifiedHtml = computed(() =>
  DOMPurify.sanitize(html.value, props.sanitize)
);
</script>
