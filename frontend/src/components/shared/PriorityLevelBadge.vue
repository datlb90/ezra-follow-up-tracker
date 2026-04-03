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
  <span
    class="inline-block cursor-default rounded-full px-2.5 py-0.5 text-xs font-medium"
    :class="styles[props.level]"
    :title="props.reason"
  >
    {{ labels[props.level] }}
  </span>
</template>
