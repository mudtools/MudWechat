/** Unix 秒 → 本地时间字符串（后端**所有时间列都是 long 秒**，格式化只在前端做）。 */
export function formatUnix(seconds: number | null | undefined): string {
  if (!seconds || seconds <= 0) {
    return '—'
  }

  return new Date(seconds * 1000).toLocaleString('zh-CN', { hour12: false })
}

/** 部门状态：1 正常 / 2 疑似删除 / 3 已删除。 */
export function deptStatusText(status: number): string {
  return status === 3 ? '已删除' : status === 2 ? '疑似删除' : '正常'
}

export function deptStatusType(status: number): 'success' | 'warning' | 'info' {
  return status === 3 ? 'info' : status === 2 ? 'warning' : 'success'
}

/** 企微成员状态：1 已激活 / 2 已禁用 / 4 未激活 / 5 退出企业。 */
export function memberStatusText(status: number | null): string {
  switch (status) {
    case 1:
      return '已激活'
    case 2:
      return '已禁用'
    case 4:
      return '未激活'
    case 5:
      return '退出企业'
    default:
      return '未知（未取到）'
  }
}

/** 任务状态：0 排队 / 1 运行 / 2 成功 / 3 失败。 */
export function runStatusText(status: number): string {
  switch (status) {
    case 0:
      return '排队'
    case 1:
      return '运行中'
    case 2:
      return '成功'
    case 3:
      return '失败'
    default:
      return '未知'
  }
}

export function runStatusType(status: number): 'info' | 'warning' | 'success' | 'danger' {
  switch (status) {
    case 1:
      return 'warning'
    case 2:
      return 'success'
    case 3:
      return 'danger'
    default:
      return 'info'
  }
}

/** 模板状态：1 正常 / 2 远端不存在。 */
export function templateStatusText(status: number): string {
  return status === 2 ? '远端不存在' : '正常'
}

export function templateStatusType(status: number): 'success' | 'info' {
  return status === 2 ? 'info' : 'success'
}

export function discoverySourceText(source: string): string {
  switch (source) {
    case 'manual':
      return '人工登记'
    case 'approval_callback':
      return '审批回调'
    case 'approval_detail':
      return '审批单回捞'
    default:
      return source
  }
}

/**
 * 「参与打印」列：官方字段是 `un_print`，语义**反直觉** —— 1 = 不参与打印、0 = 参与打印。
 * 界面上必须取反显示，否则列头与数据刚好相反。
 */
export function printFlagText(unPrint: number | null): string {
  if (unPrint === null || unPrint === undefined) {
    return '未声明'
  }

  return unPrint === 1 ? '不参与' : '参与'
}

/** 必填标记：`require` 是 int?（1 必填 / 0 非必填），不是 bool。 */
export function requireText(require: number | null): string {
  if (require === null || require === undefined) {
    return '未声明'
  }

  return require === 1 ? '必填' : '非必填'
}
