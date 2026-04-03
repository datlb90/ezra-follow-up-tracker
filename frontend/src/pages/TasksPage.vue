<script setup lang="ts">
import { onMounted, ref } from 'vue'

import LoadingSpinner from '@/components/shared/LoadingSpinner.vue'
import TaskActivityPanel from '@/components/tasks/TaskActivityPanel.vue'
import TaskFilters from '@/components/tasks/TaskFilters.vue'
import TaskRow from '@/components/tasks/TaskRow.vue'
import { useTaskStore } from '@/stores/taskStore'
import type { TaskFilters as TaskFiltersType } from '@/types/api'

const store = useTaskStore()
const activityTaskId = ref<string | null>(null)

function onFilter(filters: TaskFiltersType) {
  store.fetchTasks(filters)
}

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

    <p v-else-if="store.tasks.length === 0" class="mt-6 text-sm text-slate-400">
      No tasks found. Create one from the Findings page.
    </p>

    <div v-else class="mt-4 space-y-3">
      <TaskRow
        v-for="task in store.tasks"
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
