import type { FindingResponse, ReportResponse } from '@/types/api'

import http from './http'

export async function getReports(): Promise<ReportResponse[]> {
  const { data } = await http.get<ReportResponse[]>('/reports')
  return data
}

export async function getReportFindings(reportId: string): Promise<FindingResponse[]> {
  const { data } = await http.get<FindingResponse[]>(`/reports/${reportId}/findings`)
  return data
}
