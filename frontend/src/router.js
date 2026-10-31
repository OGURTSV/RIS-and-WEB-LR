import { createRouter, createWebHistory } from 'vue-router'

import TaskList from './pages/TaskList.vue'
import TaskNew from './pages/TaskNew.vue'
import Login from './pages/Login.vue'

// Маршруты Vue — это адреса страниц в браузере, а не запросы к серверу:
// /new только показывает форму, задачу создаст POST /api/tasks.
const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/', name: 'tasks', component: TaskList },
    { path: '/new', name: 'task-new', component: TaskNew },
    { path: '/login', name: 'login', component: Login },
  ],
})

export default router
