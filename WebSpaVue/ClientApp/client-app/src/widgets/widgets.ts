import { Plugin } from "vue";
import { wildcardImportVueComponents } from "../helpers/ImportHelpers";

const widgets: Plugin = (app) => {
  wildcardImportVueComponents(
    app,
    import.meta.globEager("./*.vue"),
    (name) => `w-${name}`
  );
};

export default widgets;
