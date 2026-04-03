<script setup lang="ts">
import type { FollowUpTaskStatus } from '@/types/api'

const props = defineProps<{ status: FollowUpTaskStatus }>()

const styles: Record<FollowUpTaskStatus, string> = {
  NotStarted: 'bg-slate-100 text-slate-600',
  InProgress: 'bg-blue-50 text-blue-700',
  Completed: 'bg-green-50 text-green-700'
}

const labels: Record<FollowUpTaskStatus, string> = {
  NotStarted: 'Not Started',
  InProgress: 'In Progress',
  Completed: 'Completed'
}
</script>

<template>
  <span
    class="inline-block rounded-full px-2.5 py-0.5 text-xs font-medium"
    :class="styles[props.status]"
  >
    {{ labels[props.status] }}
  </span>
</template>
