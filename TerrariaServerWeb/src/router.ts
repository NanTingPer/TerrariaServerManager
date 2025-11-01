import { createMemoryHistory, createRouter, type RouteRecordRaw } from 'vue-router'
import System from './components/System.vue';

const routes : RouteRecordRaw[] = [
  { path: '/', component: System },
]

const router = createRouter({
  history: createMemoryHistory(),
  routes,
})

export default router;