import type { AuthResponse, LoginRequest, RegisterRequest, UserResponse } from '@/types/api'

import http from './http'

export async function login(request: LoginRequest): Promise<AuthResponse> {
  const { data } = await http.post<AuthResponse>('/auth/login', request)
  return data
}

export async function register(request: RegisterRequest): Promise<AuthResponse> {
  const { data } = await http.post<AuthResponse>('/auth/register', request)
  return data
}

export async function getMe(): Promise<UserResponse> {
  const { data } = await http.get<UserResponse>('/auth/me')
  return data
}
