<script setup lang="ts">
import { onMounted, ref } from 'vue'

import { getTaskActivities } from '@/api/followUpTasksApi'
import LoadingSpinner from '@/components/shared/LoadingSpinner.vue'
import type { TaskActivityResponse } from '@/types/api'

const props = defineProps<{ taskId: string }>()
defineEmits<{ close: [] }>()

const activities = ref<TaskActivityResponse[]>([])
const loading = ref(true)
const error = ref<string | null>(null)

onMounted(async () => {
  try {
    activities.value = await getTaskActivities(props.taskId)
  } catch {
    error.value = 'Failed to load activities.'
  } finally {
    loading.value = false
  }
})
</script>

<template>
  <div
    class="fixed inset-0 z-50 flex items-center justify-center bg-black/30"
    role="dialog"
    aria-modal="true"
    aria-labelledby="activity-panel-heading"
    @click.self="$emit('close')"
  >
    <div class="w-full max-w-md rounded-lg bg-white p-6 shadow-lg">
      <div class="flex items-center justify-between">
        <h3 id="activity-panel-heading" class="text-base font-semibold text-slate-800">Activity History</h3>
        <button
          class="text-slate-400 hover:text-slate-600 text-sm"
          @click="$emit('close')"
        >
          Close
        </button>
      </div>

      <LoadingSpinner v-if="loading" />

      <p v-else-if="error" class="mt-4 text-sm text-red-600">{{ error }}</p>

      <p v-else-if="activities.length === 0" class="mt-4 text-sm text-slate-400">
        No activity recorded yet.
      </p>

      <ul v-else class="mt-4 space-y-3">
        <li
          v-for="activity in activities"
          :key="activity.id"
          class="border-l-2 border-slate-200 pl-3"
        >
          <p class="text-sm text-slate-700">{{ activity.summary }}</p>
          <p class="text-xs text-slate-400">
            {{ new Date(activity.occurredAt).toLocaleString() }}
          </p>
        </li>
      </ul>
    </div>
  </div>
</template>
