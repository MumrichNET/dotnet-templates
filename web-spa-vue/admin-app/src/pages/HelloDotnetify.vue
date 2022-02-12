<template>
  <div class="hello">
    <h1>{{ msg }}</h1>
    <div>
      <h3>{{ state.Greetings }}</h3>
      <p>Server time is: {{ state.ServerTime }}</p>
    </div>
  </div>
</template>

<script>
/* eslint-disable @typescript-eslint/no-unsafe-call */
/* eslint-disable @typescript-eslint/no-unsafe-member-access */
import dotnetify from 'dotnetify/vue';

dotnetify.hubServerUrl = 'https://localhost:7189';

export default dotnetify.vue.component(
  {
    name: 'hello-dotnetify',
    props: { msg: String },
  },
  'HelloDotnetify'
);
</script>
