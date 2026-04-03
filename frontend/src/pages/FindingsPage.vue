<script setup lang="ts">
import { onMounted, ref } from 'vue'

import { getReportFindings, getReports } from '@/api/reportsApi'
import CreateTaskModal from '@/components/findings/CreateTaskModal.vue'
import FindingCard from '@/components/findings/FindingCard.vue'
import LoadingSpinner from '@/components/shared/LoadingSpinner.vue'
import type { FindingResponse, ReportResponse } from '@/types/api'

const report = ref<ReportResponse | null>(null)
const findings = ref<FindingResponse[]>([])
const loading = ref(true)
const error = ref<string | null>(null)
const selectedFinding = ref<FindingResponse | null>(null)
const successMessage = ref<string | null>(null)

async function loadFindings() {
  loading.value = true
  error.value = null
  try {
    const reports = await getReports()
    if (reports.length === 0) {
      error.value = 'No reports found.'
      return
    }
    report.value = reports[0]
    findings.value = await getReportFindings(report.value.id)
  } catch {
    error.value = 'Failed to load findings.'
  } finally {
    loading.value = false
  }
}

function onTaskCreated() {
  selectedFinding.value = null
  successMessage.value = 'Task created successfully.'
  setTimeout(() => { successMessage.value = null }, 3000)
}

onMounted(loadFindings)
</script>

<template>
  <div>
    <h2 class="text-xl font-semibold text-slate-800">Findings</h2>
    <p class="mt-1 text-sm text-slate-500">
      Review findings from the sample MRI screening report.
    </p>

    <p v-if="report" class="mt-1 text-xs text-slate-400">
      {{ report.title }}
    </p>

    <div
      v-if="successMessage"
      class="mt-4 rounded-md bg-green-50 border border-green-200 px-4 py-2 text-sm text-green-700"
    >
      {{ successMessage }}
    </div>

    <LoadingSpinner v-if="loading" />

    <p v-else-if="error" class="mt-6 text-sm text-red-600">{{ error }}</p>

    <p v-else-if="findings.length === 0" class="mt-6 text-sm text-slate-400">
      No findings available.
    </p>

    <div v-else class="mt-6 space-y-3">
      <FindingCard
        v-for="finding in findings"
        :key="finding.id"
        :finding="finding"
        @create-task="selectedFinding = $event"
      />
    </div>

    <CreateTaskModal
      v-if="selectedFinding"
      :finding="selectedFinding"
      @close="selectedFinding = null"
      @created="onTaskCreated"
    />
  </div>
</template>
