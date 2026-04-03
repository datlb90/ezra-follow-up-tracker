export type FollowUpTaskStatus = 'NotStarted' | 'InProgress' | 'Completed'

export type TaskPriority = 'Low' | 'Medium' | 'High'

export type ActivityType = 'TaskCreated' | 'StatusChanged' | 'TaskUpdated'

export interface ReportResponse {
  id: string
  title: string
  receivedAt: string
  findingCount: number
}

export interface FindingResponse {
  id: string
  reportId: string
  title: string
  description: string
}

export interface FollowUpTaskResponse {
  id: string
  findingId: string
  title: string
  description: string | null
  status: FollowUpTaskStatus
  priority: TaskPriority
  createdAt: string
  updatedAt: string
}

export interface CreateFollowUpTaskRequest {
  findingId: string
  title: string
  description?: string
  priority?: TaskPriority
}

export interface UpdateFollowUpTaskRequest {
  title?: string
  description?: string
  status?: FollowUpTaskStatus
  priority?: TaskPriority
}

export interface DashboardSummaryResponse {
  totalTasks: number
  notStarted: number
  inProgress: number
  completed: number
}

export interface TaskActivityResponse {
  id: string
  followUpTaskId: string
  occurredAt: string
  type: ActivityType
  summary: string
}

export interface TaskFilters {
  status?: FollowUpTaskStatus
  priority?: TaskPriority
  search?: string
}
