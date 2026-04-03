import { createRouter, createWebHistory } from 'vue-router'

import DashboardPage from '@/pages/DashboardPage.vue'
import FindingsPage from '@/pages/FindingsPage.vue'
import TasksPage from '@/pages/TasksPage.vue'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/', name: 'dashboard', component: DashboardPage },
    { path: '/findings', name: 'findings', component: FindingsPage },
    { path: '/tasks', name: 'tasks', component: TasksPage }
  ]
})

export default router
