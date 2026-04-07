<script setup lang="ts">
import { useRouter } from 'vue-router'

import { useAuthStore } from '@/stores/authStore'

const authStore = useAuthStore()
const router = useRouter()

function onLogout() {
  authStore.logout()
  router.push('/login')
}
</script>

<template>
  <header class="h-14 border-b border-slate-200 bg-white px-6 flex items-center justify-between">
    <h1 class="text-lg font-semibold tracking-tight text-slate-800">
      Ezra Follow-Up Tracker
    </h1>
    <div v-if="authStore.isAuthenticated" class="flex items-center gap-3">
      <span class="text-sm text-slate-600">{{ authStore.user?.fullName }}</span>
      <button
        class="rounded-md border border-slate-300 px-3 py-1 text-xs font-medium text-slate-600 hover:bg-slate-50 transition-colors"
        @click="onLogout"
      >
        Sign out
      </button>
    </div>
  </header>
</template>
