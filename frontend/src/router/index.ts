import { createRouter, createWebHistory } from 'vue-router'

import DashboardPage from '@/pages/DashboardPage.vue'
import FindingsPage from '@/pages/FindingsPage.vue'
import LoginPage from '@/pages/LoginPage.vue'
import RegisterPage from '@/pages/RegisterPage.vue'
import TasksPage from '@/pages/TasksPage.vue'

declare module 'vue-router' {
  interface RouteMeta {
    isAuthPage?: boolean
  }
}

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/', name: 'dashboard', component: DashboardPage },
    { path: '/findings', name: 'findings', component: FindingsPage },
    { path: '/tasks', name: 'tasks', component: TasksPage },
    { path: '/login', name: 'login', component: LoginPage, meta: { isAuthPage: true } },
    { path: '/register', name: 'register', component: RegisterPage, meta: { isAuthPage: true } }
  ]
})

export default router
