export type FollowUpTaskStatus = 'NotStarted' | 'InProgress' | 'Completed'

export type FindingSeverity = 'Low' | 'Medium' | 'High'

export type TaskPriorityLevel = 'Low' | 'Medium' | 'High' | 'Critical'

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
  severity: FindingSeverity | null
}

export interface FollowUpTaskResponse {
  id: string
  findingId: string
  title: string
  description: string | null
  status: FollowUpTaskStatus
  dueAt: string | null
  createdAt: string
  updatedAt: string
  priorityScore: number
  priorityLevel: TaskPriorityLevel
  priorityReason: string
}

export interface CreateFollowUpTaskRequest {
  findingId: string
  title: string
  description?: string
  dueAt?: string
}

export interface UpdateFollowUpTaskRequest {
  title?: string
  description?: string
  status?: FollowUpTaskStatus
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
  actorId: string | null
  actorName: string | null
}

export interface TaskFilters {
  status?: FollowUpTaskStatus
  priorityLevel?: TaskPriorityLevel
  search?: string
}

export interface LoginRequest {
  email: string
  password: string
}

export interface RegisterRequest {
  email: string
  fullName: string
  password: string
}

export interface AuthResponse {
  token: string
  email: string
  fullName: string
}

export interface UserResponse {
  id: string
  email: string
  fullName: string
  createdAt: string
}
