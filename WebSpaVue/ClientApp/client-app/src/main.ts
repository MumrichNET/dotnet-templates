import "virtual:windi.css";
import App from "./App.vue";
import pinia from "./plugins/Pinia";
import router from "./routes/router";
import vesselize from "./plugins/Vesselize";
import { createApp } from "vue";

const app = createApp(App);

app.use(pinia);
app.use(router);
app.use(vesselize);

app.mount("#app");
