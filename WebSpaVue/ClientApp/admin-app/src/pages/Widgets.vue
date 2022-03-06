<template>
  <div>
    <q-splitter horizontal v-model="editVsPreviewSplitterModel">
      <template v-slot:before>
        <q-splitter v-model="treeVsPropsSplitterModel">
          <template v-slot:before>
            <div class="q-pa-md">
              <q-tree
                :nodes="tree"
                node-key="path"
                label-key="title"
                v-model:selected="selected"
                default-expand-all
              />
            </div>
          </template>
          <template v-slot:after>
            <div class="q-pa-md">
              <markdown
                v-if="selectedWidget?.component === 'w-markdown'"
                :is="selectedWidget.component"
                v-bind:markdown="selectedWidget.props.markdown"
              />
            </div>
          </template>
        </q-splitter>
      </template>
      <template v-slot:after>
        <div class="q-pa-md">GUGUS...</div>
      </template>
    </q-splitter>
  </div>
</template>

<script lang="ts" setup>
import { computed, ref } from 'vue';
import Markdown from 'src/widgets/Markdown.vue';
import router, { routes } from '../../../client-app/src/routes/router';
import { RouteRecordRaw } from 'vue-router';

const selected = ref<string | null>(null);
const treeVsPropsSplitterModel = ref(30);
const editVsPreviewSplitterModel = ref(50);
type TreeNode = {
  path: string;
  title: string;
  icon: string;
  children?: TreeNode[];
};

type VueWidget = { component: string; props: Record<string, any> };

function getWidgetNodes(
  path: string,
  widgets: VueWidget[]
): TreeNode[] {
  return widgets.map((w, i) => {
    return {
      path: `${path};${i}`,
      icon: 'preview',
      title: w.component,
    };
  });
}

function getRouteNodes(routes: RouteRecordRaw[], basePath?: string): TreeNode[] {
  return routes.map((r) => {
    const title = r.meta?.title as string;
    const fullPath = basePath !== undefined ? `${basePath}/${r.path}` : r.path;
    const pageChildren = r.children ? getRouteNodes(r.children, fullPath) : [];
    const widgetChildren = r.meta?.widgets
      ? getWidgetNodes(fullPath, r.meta?.widgets)
      : [];

    return {
      icon: 'web',
      path: fullPath,
      title,
      children: [...pageChildren, ...widgetChildren],
    };
  });
}

function getSelectionWidget(fullPath?: string | null): VueWidget | null {
  if (!fullPath) {
    return null;
  }

  const parts = fullPath.split(';');
  const path = parts[0];
  const route = router.getRoutes().find((r) => r.path === path) ?? null;
  const widgetIndex = parts.length > 1 ? parts[1] : null;

  if (widgetIndex !== null && route !== null) {
    return route.meta.widgets[widgetIndex];
  }

  return null;
}

const tree = computed(() => getRouteNodes(routes));
const selectedWidget = computed(() => getSelectionWidget(selected.value))
</script>
