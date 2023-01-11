<template>
  <div class="q-pa-md row items-start q-gutter-md">
    <q-card v-for="imageUrl in imageUrls" :key="imageUrl" class="image-wrapper">
      <q-img :src="imageUrl">
        <div class="absolute-bottom text-subtitle1 text-center">Caption</div>
      </q-img>
    </q-card>
  </div>
  <q-uploader :url="uploadUrl" label="Upload" style="max-width: 300px" @uploaded="onUploaded" />
</template>

<script lang="ts" setup>
import { onMounted, ref } from 'vue';

type PhysicalFileResult = {
  fileName: string;
  contentType: string;
};

const host = window.location.origin;
const uploadUrl = ref(`${host}/assets/upload`);
const imageUrls = ref<string[]>([]);

async function loadImageUrlsAsync() {
  const response = await fetch(`${host}/assets/get`);

  if (response.ok) {
    const result = (await response.json()) as PhysicalFileResult[];

    imageUrls.value = result.map((f) => `${host}${f.fileName}`);
  }
}

async function onUploaded() {
  await loadImageUrlsAsync();
}

onMounted(async () => await loadImageUrlsAsync());
</script>

<style scoped>
.image-wrapper {
  width: 100%;
  max-width: 250px;
}
</style>
