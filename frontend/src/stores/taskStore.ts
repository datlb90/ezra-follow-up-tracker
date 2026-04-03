import { ref } from 'vue'

import { defineStore } from 'pinia'

import { createTask, getDashboardSummary, getTasks, updateTask } from '@/api/followUpTasksApi'
import type {
  CreateFollowUpTaskRequest,
  DashboardSummaryResponse,
  FollowUpTaskResponse,
  TaskFilters,
  UpdateFollowUpTaskRequest
} from '@/types/api'

export const useTaskStore = defineStore('tasks', () => {
  const tasks = ref<FollowUpTaskResponse[]>([])
  const dashboardSummary = ref<DashboardSummaryResponse | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)

  async function fetchTasks(filters?: TaskFilters) {
    loading.value = true
    error.value = null
    try {
      tasks.value = await getTasks(filters)
    } catch {
      error.value = 'Failed to load tasks.'
    } finally {
      loading.value = false
    }
  }

  async function fetchDashboardSummary() {
    try {
      dashboardSummary.value = await getDashboardSummary()
    } catch {
      error.value = 'Failed to load dashboard summary.'
    }
  }

  async function addTask(request: CreateFollowUpTaskRequest): Promise<FollowUpTaskResponse | null> {
    error.value = null
    try {
      const created = await createTask(request)
      tasks.value.unshift(created)
      return created
    } catch {
      error.value = 'Failed to create task.'
      return null
    }
  }

  async function changeTask(id: string, request: UpdateFollowUpTaskRequest): Promise<boolean> {
    error.value = null
    try {
      const updated = await updateTask(id, request)
      const index = tasks.value.findIndex(t => t.id === id)
      if (index !== -1) tasks.value[index] = updated
      return true
    } catch {
      error.value = 'Failed to update task.'
      return false
    }
  }

  return { tasks, dashboardSummary, loading, error, fetchTasks, fetchDashboardSummary, addTask, changeTask }
})
