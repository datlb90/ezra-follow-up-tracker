<script setup lang="ts">
import PriorityBadge from '@/components/shared/PriorityBadge.vue'
import StatusBadge from '@/components/shared/StatusBadge.vue'
import { useTaskStore } from '@/stores/taskStore'
import type { FollowUpTaskResponse, FollowUpTaskStatus } from '@/types/api'

const props = defineProps<{ task: FollowUpTaskResponse }>()
const emit = defineEmits<{ viewActivity: [taskId: string] }>()

const store = useTaskStore()

const statusOptions: FollowUpTaskStatus[] = ['NotStarted', 'InProgress', 'Completed']

async function onStatusChange(event: Event) {
  const newStatus = (event.target as HTMLSelectElement).value as FollowUpTaskStatus
  await store.changeTask(props.task.id, { status: newStatus })
}
</script>

<template>
  <div class="rounded-lg border border-slate-200 bg-white p-4">
    <div class="flex items-start justify-between gap-4">
      <div class="min-w-0 flex-1">
        <div class="flex items-center gap-2">
          <h3 class="text-sm font-semibold text-slate-800">{{ task.title }}</h3>
          <StatusBadge :status="task.status" />
          <PriorityBadge :priority="task.priority" />
        </div>
        <p v-if="task.description" class="mt-1 text-sm text-slate-500">{{ task.description }}</p>
        <p class="mt-1 text-xs text-slate-400">
          Created {{ new Date(task.createdAt).toLocaleDateString() }}
        </p>
      </div>

      <div class="flex shrink-0 items-center gap-2">
        <select
          :value="task.status"
          aria-label="Change status"
          class="rounded-md border border-slate-300 px-2 py-1 text-xs focus:border-slate-500 focus:outline-none focus:ring-1 focus:ring-slate-500"
          @change="onStatusChange"
        >
          <option v-for="s in statusOptions" :key="s" :value="s">{{ s }}</option>
        </select>
        <button
          class="rounded-md border border-slate-300 px-2 py-1 text-xs text-slate-600 hover:bg-slate-50 transition-colors"
          @click="$emit('viewActivity', task.id)"
        >
          Activity
        </button>
      </div>
    </div>
  </div>
</template>
