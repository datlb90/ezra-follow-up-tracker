<script setup lang="ts">
import { onMounted } from 'vue'

import LoadingSpinner from '@/components/shared/LoadingSpinner.vue'
import { useTaskStore } from '@/stores/taskStore'

const store = useTaskStore()

onMounted(() => {
  store.fetchDashboardSummary()
})

const cards = [
  { label: 'Total Tasks', key: 'totalTasks' as const, accent: 'border-l-slate-400' },
  { label: 'Not Started', key: 'notStarted' as const, accent: 'border-l-slate-300' },
  { label: 'In Progress', key: 'inProgress' as const, accent: 'border-l-blue-400' },
  { label: 'Completed', key: 'completed' as const, accent: 'border-l-green-400' }
]
</script>

<template>
  <div>
    <h2 class="text-xl font-semibold text-slate-800">Dashboard</h2>
    <p class="mt-1 text-sm text-slate-500">
      Overview of follow-up task status and progress.
    </p>

    <LoadingSpinner v-if="!store.dashboardSummary && !store.error" />

    <p v-else-if="store.error" class="mt-6 text-sm text-red-600">
      {{ store.error }}
    </p>

    <div v-else class="mt-6 grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">
      <div
        v-for="card in cards"
        :key="card.key"
        class="rounded-lg border border-slate-200 border-l-4 bg-white p-5"
        :class="card.accent"
      >
        <p class="text-sm font-medium text-slate-500">{{ card.label }}</p>
        <p class="mt-1 text-2xl font-semibold text-slate-800">
          {{ store.dashboardSummary?.[card.key] ?? 0 }}
        </p>
      </div>
    </div>
  </div>
</template>
