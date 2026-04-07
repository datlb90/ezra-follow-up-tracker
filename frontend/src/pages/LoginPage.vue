<script setup lang="ts">
import { ref } from 'vue'
import { RouterLink, useRouter } from 'vue-router'

import { useAuthStore } from '@/stores/authStore'

const router = useRouter()
const authStore = useAuthStore()

const email = ref('')
const password = ref('')
const submitting = ref(false)
const formError = ref('')

async function onSubmit() {
  formError.value = ''
  submitting.value = true

  const success = await authStore.login({
    email: email.value.trim(),
    password: password.value
  })

  submitting.value = false

  if (success) {
    router.push('/')
  } else {
    formError.value = authStore.error ?? 'Login failed.'
  }
}
</script>

<template>
  <div class="min-h-screen flex items-center justify-center bg-slate-50 px-4">
    <div class="w-full max-w-sm rounded-lg bg-white p-6 shadow-lg">
      <h1 class="text-xl font-semibold text-slate-800">Sign in to Ezra</h1>
      <p class="mt-1 text-sm text-slate-500">Enter your credentials to continue.</p>

      <div
        v-if="formError"
        class="mt-4 rounded-md border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-700"
        role="alert"
      >
        {{ formError }}
      </div>

      <form class="mt-5 space-y-4" @submit.prevent="onSubmit">
        <div>
          <label for="login-email" class="block text-sm font-medium text-slate-700">Email</label>
          <input
            id="login-email"
            v-model="email"
            type="email"
            required
            autocomplete="email"
            class="mt-1 block w-full rounded-md border border-slate-300 px-3 py-2 text-sm focus:border-slate-500 focus:outline-none focus:ring-1 focus:ring-slate-500"
          />
        </div>

        <div>
          <label for="login-password" class="block text-sm font-medium text-slate-700">Password</label>
          <input
            id="login-password"
            v-model="password"
            type="password"
            required
            autocomplete="current-password"
            class="mt-1 block w-full rounded-md border border-slate-300 px-3 py-2 text-sm focus:border-slate-500 focus:outline-none focus:ring-1 focus:ring-slate-500"
          />
        </div>

        <button
          type="submit"
          :disabled="submitting || !email.trim() || !password"
          class="w-full rounded-md bg-slate-800 px-3 py-2 text-sm font-medium text-white hover:bg-slate-700 disabled:opacity-50 transition-colors"
        >
          {{ submitting ? 'Signing in...' : 'Sign in' }}
        </button>
      </form>

      <p class="mt-4 text-center text-sm text-slate-500">
        Don't have an account?
        <RouterLink to="/register" class="font-medium text-slate-800 hover:underline">Register</RouterLink>
      </p>
    </div>
  </div>
</template>
