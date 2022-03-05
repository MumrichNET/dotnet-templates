import { createVesselize } from "@vesselize/vue";
import i18n from "../i18n/i18n";

const vesselize = createVesselize({
  providers: [
    {
      token: "i18n",
      useValue: i18n,
    },
  ],
});

export default vesselize;
