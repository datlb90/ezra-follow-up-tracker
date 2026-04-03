<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'

import LoadingSpinner from '@/components/shared/LoadingSpinner.vue'
import TaskActivityPanel from '@/components/tasks/TaskActivityPanel.vue'
import TaskFilters from '@/components/tasks/TaskFilters.vue'
import TaskRow from '@/components/tasks/TaskRow.vue'
import { useTaskStore } from '@/stores/taskStore'
import type { TaskFilters as TaskFiltersType, TaskPriorityLevel } from '@/types/api'

const store = useTaskStore()
const activityTaskId = ref<string | null>(null)
const activePriorityFilter = ref<TaskPriorityLevel | undefined>()
const hasActiveFilters = ref(false)

function onFilter(filters: TaskFiltersType) {
  activePriorityFilter.value = filters.priorityLevel
  hasActiveFilters.value = !!(filters.status || filters.priorityLevel || filters.search)
  store.fetchTasks({ status: filters.status, search: filters.search })
}

const filteredTasks = computed(() => {
  if (!activePriorityFilter.value) return store.tasks
  return store.tasks.filter(t => t.priorityLevel === activePriorityFilter.value)
})

onMounted(() => {
  store.fetchTasks()
})
</script>

<template>
  <div>
    <h2 class="text-xl font-semibold text-slate-800">Tasks</h2>
    <p class="mt-1 text-sm text-slate-500">
      Manage follow-up tasks created from findings.
    </p>

    <div class="mt-4">
      <TaskFilters @filter="onFilter" />
    </div>

    <p v-if="store.error" class="mt-4 text-sm text-red-600">{{ store.error }}</p>

    <LoadingSpinner v-if="store.loading" />

    <p v-else-if="filteredTasks.length === 0" class="mt-6 text-sm text-slate-400">
      {{ hasActiveFilters ? 'No tasks match the current filters.' : 'No tasks found. Create one from the Findings page.' }}
    </p>

    <div v-else class="mt-4 space-y-3">
      <TaskRow
        v-for="task in filteredTasks"
        :key="task.id"
        :task="task"
        @view-activity="activityTaskId = $event"
      />
    </div>

    <TaskActivityPanel
      v-if="activityTaskId"
      :task-id="activityTaskId"
      @close="activityTaskId = null"
    />
  </div>
</template>
