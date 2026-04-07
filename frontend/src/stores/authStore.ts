import { computed, ref } from 'vue'

import { defineStore } from 'pinia'

import { getMe, login as apiLogin, register as apiRegister } from '@/api/authApi'
import type { AuthResponse, LoginRequest, RegisterRequest } from '@/types/api'

const TOKEN_KEY = 'ezra_auth_token'

export const useAuthStore = defineStore('auth', () => {
  const user = ref<AuthResponse | null>(null)
  const token = ref<string | null>(null)
  const loading = ref(false)
  const error = ref<string | null>(null)

  const isAuthenticated = computed(() => !!token.value)

  async function initialize() {
    const stored = localStorage.getItem(TOKEN_KEY)
    if (!stored) return

    token.value = stored
    try {
      const me = await getMe()
      user.value = { token: stored, email: me.email, fullName: me.fullName }
    } catch {
      token.value = null
      localStorage.removeItem(TOKEN_KEY)
    }
  }

  async function login(request: LoginRequest): Promise<boolean> {
    loading.value = true
    error.value = null
    try {
      const result = await apiLogin(request)
      token.value = result.token
      user.value = result
      localStorage.setItem(TOKEN_KEY, result.token)
      return true
    } catch (e: unknown) {
      if (isAxios401(e)) {
        error.value = 'Invalid email or password.'
      } else {
        error.value = 'Login failed. Please try again.'
      }
      return false
    } finally {
      loading.value = false
    }
  }

  async function register(request: RegisterRequest): Promise<boolean> {
    loading.value = true
    error.value = null
    try {
      const result = await apiRegister(request)
      token.value = result.token
      user.value = result
      localStorage.setItem(TOKEN_KEY, result.token)
      return true
    } catch (e: unknown) {
      if (isAxios409(e)) {
        error.value = 'An account with this email already exists.'
      } else {
        error.value = 'Registration failed. Please try again.'
      }
      return false
    } finally {
      loading.value = false
    }
  }

  function logout() {
    token.value = null
    user.value = null
    localStorage.removeItem(TOKEN_KEY)
  }

  return { user, token, loading, error, isAuthenticated, initialize, login, register, logout }
})

function isAxios401(e: unknown): boolean {
  return typeof e === 'object' && e !== null && 'response' in e &&
    typeof (e as { response?: { status?: number } }).response?.status === 'number' &&
    (e as { response: { status: number } }).response.status === 401
}

function isAxios409(e: unknown): boolean {
  return typeof e === 'object' && e !== null && 'response' in e &&
    typeof (e as { response?: { status?: number } }).response?.status === 'number' &&
    (e as { response: { status: number } }).response.status === 409
}
