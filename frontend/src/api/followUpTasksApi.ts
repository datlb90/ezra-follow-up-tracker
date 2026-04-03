import type {
  CreateFollowUpTaskRequest,
  DashboardSummaryResponse,
  FollowUpTaskResponse,
  TaskActivityResponse,
  TaskFilters,
  UpdateFollowUpTaskRequest
} from '@/types/api'

import http from './http'

export async function getTasks(filters?: TaskFilters): Promise<FollowUpTaskResponse[]> {
  const { data } = await http.get<FollowUpTaskResponse[]>('/follow-up-tasks', {
    params: filters
  })
  return data
}

export async function getTaskById(id: string): Promise<FollowUpTaskResponse> {
  const { data } = await http.get<FollowUpTaskResponse>(`/follow-up-tasks/${id}`)
  return data
}

export async function createTask(request: CreateFollowUpTaskRequest): Promise<FollowUpTaskResponse> {
  const { data } = await http.post<FollowUpTaskResponse>('/follow-up-tasks', request)
  return data
}

export async function updateTask(id: string, request: UpdateFollowUpTaskRequest): Promise<FollowUpTaskResponse> {
  const { data } = await http.put<FollowUpTaskResponse>(`/follow-up-tasks/${id}`, request)
  return data
}

export async function getDashboardSummary(): Promise<DashboardSummaryResponse> {
  const { data } = await http.get<DashboardSummaryResponse>('/follow-up-tasks/dashboard')
  return data
}

export async function getTaskActivities(taskId: string): Promise<TaskActivityResponse[]> {
  const { data } = await http.get<TaskActivityResponse[]>(`/follow-up-tasks/${taskId}/activities`)
  return data
}
