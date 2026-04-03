<script setup lang="ts">
import type { FindingSeverity } from '@/types/api'

const props = defineProps<{ severity: FindingSeverity | null }>()

const styles: Record<string, string> = {
  Low: 'bg-slate-100 text-slate-500',
  Medium: 'bg-amber-50 text-amber-700',
  High: 'bg-red-50 text-red-700'
}
</script>

<template>
  <span
    v-if="props.severity"
    class="inline-block rounded-full px-2.5 py-0.5 text-xs font-medium"
    :class="styles[props.severity]"
  >
    {{ props.severity }}
  </span>
</template>
