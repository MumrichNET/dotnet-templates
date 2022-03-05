import "vue-router";

type VueWidget = { component: string; props: Record<string, any> };

declare module "vue-router" {
  interface RouteMeta {
    widgets?: VueWidget[];
    title?: string;
  }
}
