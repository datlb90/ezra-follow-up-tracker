<script setup lang="ts">
import { ref } from 'vue'

import { useTaskStore } from '@/stores/taskStore'
import type { FindingResponse } from '@/types/api'

const props = defineProps<{ finding: FindingResponse }>()
const emit = defineEmits<{ close: []; created: [] }>()

const store = useTaskStore()

const title = ref(props.finding.title)
const description = ref('')
const dueAt = ref('')
const submitting = ref(false)
const titleError = ref('')
const formError = ref('')

async function onSubmit() {
  titleError.value = ''
  formError.value = ''

  if (!title.value.trim()) {
    titleError.value = 'Title is required.'
    return
  }

  submitting.value = true
  const result = await store.addTask({
    findingId: props.finding.id,
    title: title.value.trim(),
    description: description.value.trim() || undefined,
    dueAt: dueAt.value || undefined
  })
  submitting.value = false

  if (result) {
    emit('created')
  } else {
    formError.value = store.error ?? 'Failed to create task.'
  }
}
</script>

<template>
  <div
    class="fixed inset-0 z-50 flex items-center justify-center bg-black/30"
    role="dialog"
    aria-modal="true"
    aria-labelledby="create-task-heading"
    @click.self="$emit('close')"
  >
    <div class="w-full max-w-md rounded-lg bg-white p-6 shadow-lg">
      <h3 id="create-task-heading" class="text-base font-semibold text-slate-800">Create Follow-Up Task</h3>
      <p class="mt-1 text-xs text-slate-500">From finding: {{ finding.title }}</p>

      <div
        v-if="formError"
        class="mt-3 rounded-md border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-700"
        role="alert"
      >
        {{ formError }}
      </div>

      <form class="mt-4 space-y-4" @submit.prevent="onSubmit">
        <div>
          <label for="task-title" class="block text-sm font-medium text-slate-700">Title</label>
          <input
            id="task-title"
            v-model="title"
            type="text"
            required
            maxlength="200"
            class="mt-1 block w-full rounded-md border px-3 py-2 text-sm focus:outline-none focus:ring-1"
            :class="titleError
              ? 'border-red-400 focus:border-red-500 focus:ring-red-500'
              : 'border-slate-300 focus:border-slate-500 focus:ring-slate-500'"
            :aria-invalid="!!titleError"
            :aria-describedby="titleError ? 'task-title-error' : undefined"
          />
          <p v-if="titleError" id="task-title-error" class="mt-1 text-xs text-red-600">{{ titleError }}</p>
        </div>

        <div>
          <label for="task-description" class="block text-sm font-medium text-slate-700">Description</label>
          <textarea
            id="task-description"
            v-model="description"
            rows="2"
            maxlength="2000"
            class="mt-1 block w-full rounded-md border border-slate-300 px-3 py-2 text-sm focus:border-slate-500 focus:outline-none focus:ring-1 focus:ring-slate-500"
          />
        </div>

        <div>
          <label for="task-due-date" class="block text-sm font-medium text-slate-700">Due Date</label>
          <input
            id="task-due-date"
            v-model="dueAt"
            type="date"
            class="mt-1 block w-full rounded-md border border-slate-300 px-3 py-2 text-sm focus:border-slate-500 focus:outline-none focus:ring-1 focus:ring-slate-500"
          />
        </div>

        <div class="flex justify-end gap-2 pt-2">
          <button
            type="button"
            class="rounded-md border border-slate-300 px-3 py-1.5 text-sm font-medium text-slate-600 hover:bg-slate-50 transition-colors"
            :disabled="submitting"
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
