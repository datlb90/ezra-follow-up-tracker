<script setup lang="ts">
import type { TaskPriorityLevel } from '@/types/api'

const props = defineProps<{ level: TaskPriorityLevel; reason?: string }>()

const styles: Record<TaskPriorityLevel, string> = {
  Low: 'bg-slate-100 text-slate-500',
  Medium: 'bg-blue-50 text-blue-700',
  High: 'bg-orange-50 text-orange-700',
  Critical: 'bg-red-100 text-red-800'
}

const labels: Record<TaskPriorityLevel, string> = {
  Low: 'Low',
  Medium: 'Medium',
  High: 'High',
  Critical: 'Critical'
}
</script>

<template>
  <span class="priority-badge relative inline-block cursor-default">
    <span
      class="inline-block rounded-full px-2.5 py-0.5 text-xs font-medium"
      :class="styles[props.level]"
    >
      {{ labels[props.level] }}
    </span>
    <span
      v-if="props.reason"
      class="priority-tooltip pointer-events-none absolute left-0 top-full z-50 mt-1.5 whitespace-nowrap rounded border border-slate-200 bg-white px-2.5 py-1.5 text-xs text-slate-700 opacity-0 shadow-md"
    >
      {{ props.reason }}
    </span>
  </span>
</template>

<style scoped>
.priority-badge:hover .priority-tooltip {
  opacity: 1;
}
</style>
