import { RouteRecordRaw } from 'vue-router';

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    component: () => import('layouts/MainLayout.vue'),
    children: [
      { path: '', component: () => import('pages/IndexPage.vue') },
      {
        path: 'login',
        component: () => import('src/pages/LoginPage.vue'),
      },
      {
        path: 'student',
        component: () => import('pages/Student/StudentPage.vue'),
      },
      {
        path: 'student/create',
        component: () => import('src/pages/Student/StudentCreate.vue'),
      },
      {
        path: 'approval',
        component: () => import('pages/Approval/ApprovalPage.vue'),
      },
      {
        path: 'approval/:id',
        component: () => import('src/pages/Approval/ApprovalModal.vue'),
      },
      {
        path: 'admin',
        component: () => import('pages/Admin/AdminDashboard.vue'),
      },
      {
        path: 'admin/stage',
        component: () => import('pages/Admin/AdminPage.vue'),
      },
    ],
  },

  // Always leave this as last one,
  // but you can also remove it
  {
    path: '/:catchAll(.*)*',
    component: () => import('pages/ErrorNotFound.vue'),
  },
];

export default routes;
