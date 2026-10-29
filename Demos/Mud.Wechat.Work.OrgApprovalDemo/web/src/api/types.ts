/** 后端投影 DTO（方案 §4.4）的一一对应声明。字段名与后端匿名对象的 camelCase 一致。 */

export interface HealthInfo {
  status: string
  demo: string
  utcNow: string
  writeGate: 'enabled' | 'disabled'
  config: {
    apps: { appKey: string; secretTail: string }[]
    modules: string[]
    callback: { enabled: boolean; route: string; hint: string }
    discovery: { enabled: boolean; hint: string }
    store: {
      provider: string
      path: string
      tables: Record<string, number>
      hint: string
    }
  }
}

/** 部门树的一行；`orderNo` 是**值小靠前**（与成员 order 相反）。 */
export interface DeptNode {
  id: number
  name: string | null
  parentId: number
  orderNo: number
  leaders: string[]
  memberCount: number
  childCount: number
  status: number
  syncedAt: number
}

export interface DeptDetail {
  id: number
  name: string | null
  nameEn: string | null
  parentId: number
  orderNo: number
  leaders: string[]
  sourceHash: string | null
  syncedAt: number
  localEditable: boolean
}

export interface MemberRow {
  userId: string
  name: string | null
  position: string | null
  mainDeptId: number | null
  deptIds: number[]
  isLeader: boolean
  status: number | null
  localStatus: number
  mobileMasked: string | null
  emailMasked: string | null
  syncedAt: number
}

export interface MemberDetail {
  userId: string
  name: string | null
  position: string | null
  gender: string | null
  mobile: string | null
  email: string | null
  mainDeptId: number | null
  deptIds: number[]
  leaderFlags: number[]
  status: number | null
  localStatus: number
  sourceHash: string | null
  syncedAt: number
}

export interface SyncRunView {
  id: number
  kind: string
  status: number
  startedAt: number
  finishedAt: number
  deptCount: number
  userCount: number
  processed: number
  total: number
  phase: string | null
  errorCode: number
  errorMessage: string | null
  hasDiff: boolean
}

export interface EntityRef {
  kind: string
  id: string
  name: string | null
}

export interface OrgDiff {
  created: EntityRef[]
  updated: EntityRef[]
  missing: EntityRef[]
  counts: Record<string, number>
  guardian: string | null
}

export interface TemplateRow {
  templateId: string
  name: string | null
  nameEn: string | null
  controlCount: number
  version: number
  lastSyncedAt: number
  status: number
  discoverySource: string
}

export interface ControlNode {
  controlId: string
  control: string | null
  title: string | null
  require: number | null
  unPrint: number | null
  orderNo: number
  config: unknown
  children: ControlNode[]
}

export interface TemplateDetail {
  templateId: string
  name: string | null
  nameEn: string | null
  version: number
  status: number
  discoverySource: string
  lastSyncedAt: number
  controlCount: number
  controls: ControlNode[]
  rawJson: string
}

export interface TemplateVersionRow {
  version: number
  captureAt: number
  changeSummary: string | null
  contentHash: string
}

export interface FieldDelta {
  field: string
  before: string | null
  after: string | null
}

export interface ControlDiff {
  controlId: string
  kind: 'add' | 'remove' | 'change'
  title: string | null
  fields: FieldDelta[]
}

export interface TemplateDiffResult {
  templateId?: string | null
  from?: number | null
  to?: number | null
  added: ControlDiff[]
  removed: ControlDiff[]
  changed: ControlDiff[]
  hint?: string
}

export interface RegistryRow {
  templateId: string
  status: string
  firstSeenAt: number
  discoveredFrom: string
  note: string | null
}

export interface ChangeLogRow {
  id: number
  source: string
  entityType: string
  entityId: string
  kind: string
  occurredAt: number
  applied: number
}

/**
 * 提交给 `create_template` / `update_template` / `dry-run` 的控件（请求侧类型）。
 *
 * **不能把响应侧的控件对象直接回写**：响应侧 `config` 与请求侧 `config` 是两个类型
 * （`ApprovalTemplateControlConfig` vs `ApprovalTemplateSettingControlConfig`），
 * 结构不等宽。本 Demo 的编辑页以请求侧结构为准。
 */
export interface SettingControl {
  property: {
    control: string | null
    id: string | null
    title: { text: string | null; lang: string | null }[] | null
    placeholder?: { text: string | null; lang: string | null }[] | null
    require: number | null
    un_print: number | null
  }
  config?: Record<string, unknown> | null
}
