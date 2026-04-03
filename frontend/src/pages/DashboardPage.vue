<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'

import { getDashboardSummary } from '@/api/followUpTasksApi'
import LoadingSpinner from '@/components/shared/LoadingSpinner.vue'
import { useTaskStore } from '@/stores/taskStore'
import type { DashboardSummaryResponse } from '@/types/api'

const store = useTaskStore()

const summary = ref<DashboardSummaryResponse | null>(null)
const loading = ref(true)
const error = ref<string | null>(null)

async function loadDashboard() {
  loading.value = true
  error.value = null
  try {
    const [summaryResult] = await Promise.all([
      getDashboardSummary(),
      store.fetchTasks()
    ])
    summary.value = summaryResult
  } catch {
    error.value = 'Failed to load dashboard.'
  } finally {
    loading.value = false
  }
}

onMounted(loadDashboard)

const criticalCount = computed(() =>
  store.tasks.filter(t => t.priorityLevel === 'Critical').length
)

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

    <LoadingSpinner v-if="loading" />

    <p v-else-if="error" class="mt-6 text-sm text-red-600" role="alert">
      {{ error }}
    </p>

    <div v-else-if="summary" class="mt-6 grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-5">
      <div
        v-for="card in cards"
        :key="card.key"
        class="rounded-lg border border-slate-200 border-l-4 bg-white p-5"
        :class="card.accent"
      >
        <p class="text-sm font-medium text-slate-500">{{ card.label }}</p>
        <p class="mt-1 text-2xl font-semibold text-slate-800">
          {{ summary[card.key] }}
        </p>
      </div>
      <div class="rounded-lg border border-slate-200 border-l-4 border-l-red-400 bg-white p-5">
        <p class="text-sm font-medium text-slate-500">Critical</p>
        <p class="mt-1 text-2xl font-semibold text-slate-800">{{ criticalCount }}</p>
      </div>
    </div>
  </div>
</template>
