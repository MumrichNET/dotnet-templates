import "vue-router";

declare module "vue-router" {
  interface RouteMeta {
    widgets?: VueWidget[];
    title?: string;
  }
}
