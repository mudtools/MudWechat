import { del, get, post, put } from './client'
import type {
  ChangeLogRow,
  ControlNode,
  DeptDetail,
  DeptNode,
  HealthInfo,
  MemberDetail,
  MemberRow,
  OrgDiff,
  RegistryRow,
  SettingControl,
  SyncRunView,
  TemplateDetail,
  TemplateDiffResult,
  TemplateRow,
  TemplateVersionRow,
} from './types'

/** 健康检查：前端**只**用它判定写闸，不靠 404/405 兜底（方案 §7.3 第 1 条）。 */
export const getHealth = () => get<HealthInfo>('/healthz')

export const listDepartments = (includeDeleted = false) =>
  get<{ items: DeptNode[] }>(`/api/org/departments?includeDeleted=${includeDeleted ? 1 : 0}`)

export const getDepartment = (id: number) => get<DeptDetail>(`/api/org/departments/${id}`)

export const listMembers = (params: {
  deptId?: number
  keyword?: string
  page?: number
  size?: number
  includeDeleted?: boolean
}) => {
  const search = new URLSearchParams()
  if (params.deptId !== undefined) search.set('deptId', String(params.deptId))
  if (params.keyword) search.set('keyword', params.keyword)
  search.set('page', String(params.page ?? 1))
  search.set('size', String(params.size ?? 20))
  if (params.includeDeleted) search.set('includeDeleted', '1')
  return get<{ items: MemberRow[]; total: number }>(`/api/org/members?${search.toString()}`)
}

export const getMember = (userId: string) =>
  get<MemberDetail>(`/api/org/members/${encodeURIComponent(userId)}`)

export const listSyncRuns = (limit = 20) => get<{ items: SyncRunView[] }>(`/api/sync/runs?limit=${limit}`)

export const getSyncRun = (id: number) => get<SyncRunView>(`/api/sync/runs/${id}`)

export const getSyncDiff = (id: number) => get<OrgDiff>(`/api/sync/runs/${id}/diff`)

export const listChanges = (limit = 50) =>
  get<{ items: ChangeLogRow[] }>(`/api/sync/changes?limit=${limit}`)

export const startSync = (mode: 'full' | 'reconcile') =>
  post<{ runId: number; kind: string }>(`/api/org/sync?mode=${mode}`)

/**
 * 「疑似删除」的人工确认：**唯一**的本地物理删除入口
 * （全量同步只标 status=2，绝不物理删 —— 方案 §6.4）。
 */
export const confirmMissing = (runId: number, entityType: 'dept' | 'user', entityId: string) =>
  post<{ entityType: string; entityId: string; deleted: boolean }>(
    `/api/sync/runs/${runId}/confirm-missing`,
    { entityType, entityId },
  )

export const createDepartment = (body: {
  name: string
  nameEn?: string | null
  parentId: number
  order?: number | null
}) => post<{ id: number; name: string; parentId: number }>('/api/org/departments', body)

export const updateDepartment = (
  id: number,
  body: { name?: string | null; nameEn?: string | null; parentId?: number | null; order?: number | null },
) => put<{ id: number; updated: boolean }>(`/api/org/departments/${id}`, body)

export const deleteDepartment = (id: number) =>
  del<{ id: number; deleted: boolean }>(`/api/org/departments/${id}?confirm=${id}`)

export const createMember = (body: {
  userId: string
  name: string
  mobile?: string | null
  email?: string | null
  position?: string | null
  gender?: string | null
  departmentIds: number[]
  mainDeptId?: number | null
  isLeaderInDept?: number[] | null
}) => post<{ userId: string; createdDepartmentList: unknown[] }>('/api/org/members', body)

export const updateMember = (
  userId: string,
  body: {
    name?: string | null
    mobile?: string | null
    email?: string | null
    position?: string | null
    gender?: string | null
    departmentIds?: number[] | null
    mainDeptId?: number | null
    isLeaderInDept?: number[] | null
  },
) => put<{ userId: string; updated: boolean }>(`/api/org/members/${encodeURIComponent(userId)}`, body)

export const deleteMember = (userId: string) =>
  del<{ userId: string; deleted: boolean }>(
    `/api/org/members/${encodeURIComponent(userId)}?confirm=${encodeURIComponent(userId)}`,
  )

export const listTemplates = () =>
  get<{ items: TemplateRow[]; hint: string }>('/api/oa/templates')

export const getTemplate = (templateId: string) =>
  get<TemplateDetail>(`/api/oa/templates/${encodeURIComponent(templateId)}`)

export const listTemplateVersions = (templateId: string) =>
  get<{ items: TemplateVersionRow[] }>(`/api/oa/templates/${encodeURIComponent(templateId)}/versions`)

export const getTemplateDiff = (templateId: string, from?: number, to?: number) => {
  const search = new URLSearchParams()
  if (from !== undefined) search.set('from', String(from))
  if (to !== undefined) search.set('to', String(to))
  const suffix = search.toString() ? `?${search.toString()}` : ''
  return get<TemplateDiffResult>(`/api/oa/templates/${encodeURIComponent(templateId)}/diff${suffix}`)
}

export const listRegistry = (status?: string) =>
  get<{ items: RegistryRow[] }>(
    `/api/oa/templates/registry${status ? `?status=${encodeURIComponent(status)}` : ''}`,
  )

export const registerTemplate = (templateId: string) =>
  post<{ templateId: string; status: string }>('/api/oa/templates/registry', { templateId })

export const confirmRegistry = (templateId: string) =>
  post<{ templateId: string; status: string }>(
    `/api/oa/templates/registry/${encodeURIComponent(templateId)}/confirm`,
  )

export const ignoreRegistry = (templateId: string) =>
  post<{ templateId: string; status: string }>(
    `/api/oa/templates/registry/${encodeURIComponent(templateId)}/ignore`,
  )

export const syncOneTemplate = (templateId: string) =>
  post<{ templateId: string; createdNewVersion: boolean }>(
    `/api/oa/templates/${encodeURIComponent(templateId)}/sync`,
  )

export const startTemplateSync = () => post<{ runId: number; kind: string }>('/api/oa/templates/sync')

export const startDiscovery = () => post<{ runId: number; kind: string; hint: string }>('/api/oa/templates/discover')

export const dryRunTemplate = (templateId: string | null, controls: SettingControl[]) =>
  post<TemplateDiffResult & { hint: string }>('/api/oa/templates/dry-run', { templateId, controls })

export const createTemplate = (name: string, controls: SettingControl[]) =>
  post<{ templateId: string; hint: string }>('/api/oa/templates', { name, controls })

export const updateTemplate = (templateId: string, baseVersion: number, name: string, controls: SettingControl[]) =>
  put<{ templateId: string; version: number; hint: string }>(
    `/api/oa/templates/${encodeURIComponent(templateId)}`,
    { baseVersion, name, controls },
  )

export type { ControlNode }
