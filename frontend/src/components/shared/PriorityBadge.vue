<script setup lang="ts">
import type { TaskPriority } from '@/types/api'

const props = defineProps<{ priority: TaskPriority }>()

const styles: Record<TaskPriority, string> = {
  Low: 'bg-slate-100 text-slate-500',
  Medium: 'bg-amber-50 text-amber-700',
  High: 'bg-red-50 text-red-700'
}
</script>

<template>
  <span
    class="inline-block rounded-full px-2.5 py-0.5 text-xs font-medium"
    :class="styles[props.priority]"
  >
    {{ props.priority }}
  </span>
</template>
