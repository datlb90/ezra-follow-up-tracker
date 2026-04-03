<script setup lang="ts">
import { ref, watch } from 'vue'

import type { FollowUpTaskStatus, TaskFilters, TaskPriority } from '@/types/api'

const emit = defineEmits<{ filter: [filters: TaskFilters] }>()

const status = ref<FollowUpTaskStatus | ''>('')
const priority = ref<TaskPriority | ''>('')
const search = ref('')

function emitFilters() {
  emit('filter', {
    status: status.value || undefined,
    priority: priority.value || undefined,
    search: search.value.trim() || undefined
  })
}

watch([status, priority], emitFilters)

let debounceTimer: ReturnType<typeof setTimeout>
watch(search, () => {
  clearTimeout(debounceTimer)
  debounceTimer = setTimeout(emitFilters, 300)
})
</script>

<template>
  <div class="flex flex-wrap items-center gap-3">
    <select
      v-model="status"
      aria-label="Filter by status"
      class="rounded-md border border-slate-300 px-3 py-1.5 text-sm focus:border-slate-500 focus:outline-none focus:ring-1 focus:ring-slate-500"
    >
      <option value="">All Statuses</option>
      <option value="NotStarted">Not Started</option>
      <option value="InProgress">In Progress</option>
      <option value="Completed">Completed</option>
    </select>

    <select
      v-model="priority"
      aria-label="Filter by priority"
      class="rounded-md border border-slate-300 px-3 py-1.5 text-sm focus:border-slate-500 focus:outline-none focus:ring-1 focus:ring-slate-500"
    >
      <option value="">All Priorities</option>
      <option value="Low">Low</option>
      <option value="Medium">Medium</option>
      <option value="High">High</option>
    </select>

    <input
      v-model="search"
      type="text"
      placeholder="Search tasks..."
      aria-label="Search tasks"
      class="rounded-md border border-slate-300 px-3 py-1.5 text-sm focus:border-slate-500 focus:outline-none focus:ring-1 focus:ring-slate-500"
    />
  </div>
</template>
