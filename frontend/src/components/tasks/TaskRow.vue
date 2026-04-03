<script setup lang="ts">
import { ref } from 'vue'

import PriorityLevelBadge from '@/components/shared/PriorityLevelBadge.vue'
import { useTaskStore } from '@/stores/taskStore'
import type { FollowUpTaskResponse, FollowUpTaskStatus } from '@/types/api'

const props = defineProps<{ task: FollowUpTaskResponse }>()
defineEmits<{ viewActivity: [taskId: string] }>()

const store = useTaskStore()
const updating = ref(false)

const statusOptions: { value: FollowUpTaskStatus; label: string }[] = [
  { value: 'NotStarted', label: 'Not Started' },
  { value: 'InProgress', label: 'In Progress' },
  { value: 'Completed', label: 'Completed' }
]

const statusStyles: Record<FollowUpTaskStatus, { active: string; inactive: string }> = {
  NotStarted: {
    active: 'bg-slate-600 text-white border-slate-600',
    inactive: 'bg-white text-slate-600 border-slate-300 hover:bg-slate-50'
  },
  InProgress: {
    active: 'bg-blue-600 text-white border-blue-600',
    inactive: 'bg-white text-blue-700 border-slate-300 hover:bg-blue-50'
  },
  Completed: {
    active: 'bg-green-600 text-white border-green-600',
    inactive: 'bg-white text-green-700 border-slate-300 hover:bg-green-50'
  }
}

async function onStatusClick(newStatus: FollowUpTaskStatus) {
  if (newStatus === props.task.status || updating.value) return
  updating.value = true
  await store.changeTask(props.task.id, { status: newStatus })
  updating.value = false
}
</script>

<template>
  <div class="rounded-lg border border-slate-200 bg-white p-4">
    <div class="flex items-start justify-between gap-4">
      <div class="min-w-0 flex-1">
        <div class="flex items-center gap-2 flex-wrap">
          <h3 class="text-sm font-semibold text-slate-800">{{ task.title }}</h3>
          <PriorityLevelBadge :level="task.priorityLevel" :reason="task.priorityReason" />
        </div>
        <p v-if="task.description" class="mt-1 text-sm text-slate-500">{{ task.description }}</p>
        <p class="mt-1 text-xs text-slate-400">
          Created {{ new Date(task.createdAt).toLocaleDateString() }}
          <span v-if="task.dueAt"> &middot; Due {{ new Date(task.dueAt).toLocaleDateString() }}</span>
        </p>
      </div>

      <div class="flex shrink-0 items-center gap-2">
        <div class="inline-flex rounded-md" role="group" aria-label="Change status">
          <button
            v-for="(opt, idx) in statusOptions"
            :key="opt.value"
            :disabled="updating"
            class="border px-2.5 py-1 text-xs font-medium transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
            :class="[
              task.status === opt.value
                ? statusStyles[opt.value].active
                : statusStyles[opt.value].inactive,
              idx === 0 ? 'rounded-l-md' : '',
              idx === statusOptions.length - 1 ? 'rounded-r-md' : '',
              idx > 0 ? '-ml-px' : ''
            ]"
            @click="onStatusClick(opt.value)"
          >
            {{ opt.label }}
          </button>
        </div>
        <button
          class="rounded-md border border-slate-300 px-2 py-1 text-xs text-slate-600 hover:bg-slate-50 transition-colors"
          @click="$emit('viewActivity', task.id)"
        >
          View Activity
        </button>
      </div>
    </div>
  </div>
</template>
