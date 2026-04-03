<script setup lang="ts">
import { RouterLink, useRoute } from 'vue-router'

const route = useRoute()

const navItems = [
  { to: '/', label: 'Dashboard' },
  { to: '/findings', label: 'Findings' },
  { to: '/tasks', label: 'Tasks' }
]

function isActive(path: string): boolean {
  return route.path === path
}
</script>

<template>
  <aside class="w-56 border-r border-slate-200 bg-white flex flex-col">
    <nav class="flex-1 py-4 space-y-1 px-3">
      <RouterLink
        v-for="item in navItems"
        :key="item.to"
        :to="item.to"
        class="block rounded-md px-3 py-2 text-sm font-medium transition-colors"
        :class="isActive(item.to)
          ? 'bg-slate-100 text-slate-900'
          : 'text-slate-600 hover:bg-slate-50 hover:text-slate-900'"
      >
        {{ item.label }}
      </RouterLink>
    </nav>
  </aside>
</template>
