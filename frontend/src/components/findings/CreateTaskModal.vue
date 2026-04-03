<script setup lang="ts">
import { ref } from 'vue'

import { useTaskStore } from '@/stores/taskStore'
import type { FindingResponse, TaskPriority } from '@/types/api'

const props = defineProps<{ finding: FindingResponse }>()
const emit = defineEmits<{ close: []; created: [] }>()

const store = useTaskStore()

const title = ref(props.finding.title)
const description = ref('')
const priority = ref<TaskPriority>('Medium')
const submitting = ref(false)

async function onSubmit() {
  if (!title.value.trim()) return
  submitting.value = true
  const result = await store.addTask({
    findingId: props.finding.id,
    title: title.value.trim(),
    description: description.value.trim() || undefined,
    priority: priority.value
  })
  submitting.value = false
  if (result) emit('created')
}
</script>

<template>
  <div class="fixed inset-0 z-50 flex items-center justify-center bg-black/30" @click.self="$emit('close')">
    <div class="w-full max-w-md rounded-lg bg-white p-6 shadow-lg">
      <h3 class="text-base font-semibold text-slate-800">Create Follow-Up Task</h3>
      <p class="mt-1 text-xs text-slate-500">From finding: {{ finding.title }}</p>

      <form class="mt-4 space-y-4" @submit.prevent="onSubmit">
        <div>
          <label for="task-title" class="block text-sm font-medium text-slate-700">Title</label>
          <input
            id="task-title"
            v-model="title"
            type="text"
            required
            maxlength="200"
            class="mt-1 block w-full rounded-md border border-slate-300 px-3 py-2 text-sm focus:border-slate-500 focus:outline-none focus:ring-1 focus:ring-slate-500"
          />
        </div>

        <div>
          <label for="task-description" class="block text-sm font-medium text-slate-700">Description</label>
          <textarea
            id="task-description"
            v-model="description"
            rows="2"
            class="mt-1 block w-full rounded-md border border-slate-300 px-3 py-2 text-sm focus:border-slate-500 focus:outline-none focus:ring-1 focus:ring-slate-500"
          />
        </div>

        <div>
          <label for="task-priority" class="block text-sm font-medium text-slate-700">Priority</label>
          <select
            id="task-priority"
            v-model="priority"
            class="mt-1 block w-full rounded-md border border-slate-300 px-3 py-2 text-sm focus:border-slate-500 focus:outline-none focus:ring-1 focus:ring-slate-500"
          >
            <option value="Low">Low</option>
            <option value="Medium">Medium</option>
            <option value="High">High</option>
          </select>
        </div>

        <div class="flex justify-end gap-2 pt-2">
          <button
            type="button"
            class="rounded-md border border-slate-300 px-3 py-1.5 text-sm font-medium text-slate-600 hover:bg-slate-50 transition-colors"
            @click="$emit('close')"
          >
            Cancel
          </button>
          <button
            type="submit"
            :disabled="submitting || !title.trim()"
            class="rounded-md bg-slate-800 px-3 py-1.5 text-sm font-medium text-white hover:bg-slate-700 disabled:opacity-50 transition-colors"
          >
            {{ submitting ? 'Creating...' : 'Create Task' }}
          </button>
        </div>
      </form>
    </div>
  </div>
</template>
