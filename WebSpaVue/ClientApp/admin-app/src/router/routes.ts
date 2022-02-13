import { RouteRecordRaw } from 'vue-router';

const mainLayout = () => import('layouts/MainLayout.vue');

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    component: mainLayout,
    children: [{ path: '', component: () => import('pages/Index.vue') }],
  },
  {
    path: '/hello-dotnetify',
    component: mainLayout,
    children: [
      {
        path: '',
        component: () => import('pages/HelloDotnetify.vue'),
      },
    ],
  },
  {
    path: '/media',
    component: mainLayout,
    children: [
      {
        path: '',
        component: () => import('src/pages/Media.vue'),
      },
    ],
  },

  // Always leave this as last one,
  // but you can also remove it
  {
    path: '/:catchAll(.*)*',
    component: () => import('pages/Error404.vue'),
  },
];

export default routes;
