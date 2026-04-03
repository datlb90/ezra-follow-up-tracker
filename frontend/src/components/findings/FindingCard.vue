<script setup lang="ts">
import SeverityBadge from '@/components/shared/SeverityBadge.vue'
import type { FindingResponse } from '@/types/api'

defineProps<{ finding: FindingResponse }>()
defineEmits<{ createTask: [finding: FindingResponse] }>()
</script>

<template>
  <div class="rounded-lg border border-slate-200 bg-white p-5">
    <div class="flex items-start justify-between gap-4">
      <div class="min-w-0 flex-1">
        <div class="flex items-center gap-2">
          <h3 class="text-sm font-semibold text-slate-800">{{ finding.title }}</h3>
          <SeverityBadge :severity="finding.severity" />
        </div>
        <p class="mt-1 text-sm text-slate-500 leading-relaxed">{{ finding.description }}</p>
      </div>
      <button
        class="shrink-0 rounded-md bg-slate-800 px-3 py-1.5 text-xs font-medium text-white hover:bg-slate-700 transition-colors"
        @click="$emit('createTask', finding)"
      >
        Create Task
      </button>
    </div>
  </div>
</template>
