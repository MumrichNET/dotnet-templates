import "virtual:windi.css";
import App from "./App.vue";
import i18n from "./i18n/i18n";
import pinia from "./plugins/Pinia";
import router from "./routes/router";
import vesselize from "./plugins/Vesselize";
import widgets from "./widgets/widgets";
import { createApp } from "vue";

const app = createApp(App);

app.use(i18n);
app.use(pinia);
app.use(router);
app.use(vesselize);
app.use(widgets);

app.mount("#app");
