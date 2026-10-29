/** 官方秒级 Unix 时间戳（服务端不做时区假设，方案 §4.3）→ 本地可读时间。 */
export function formatUnix(seconds: number | null | undefined): string {
  if (!seconds || seconds <= 0) {
    return '—'
  }

  return new Date(seconds * 1000).toLocaleString()
}

/** ISO8601（本地台账里的 DateTimeOffset，含偏移）→ 本地可读时间。 */
export function formatIso(value: string | null | undefined): string {
  if (!value) {
    return '—'
  }

  return new Date(value).toLocaleString()
}

/** 取尾部若干位用于展示 id：完整 id 会诱导读者复制粘贴到线上工具里。 */
export function tail(value: string | null | undefined, length = 8): string {
  if (!value) {
    return '未选'
  }

  return value.length <= length ? value : `…${value.slice(-length)}`
}

/** publish_status 数字与中文并列（方案 §6.4：状态码原样显示，不合并语义）。 */
export function publishStatusText(status: number | null | undefined): string {
  switch (status) {
    case null:
    case undefined:
      return '未知（尚未轮询）'
    case 0:
      return '发布成功'
    case 1:
      return '发布中'
    case 2:
      return '原创失败'
    case 3:
      return '常规失败'
    case 4:
      return '平台审核不通过'
    case 5:
      return '成功后被用户删除'
    case 6:
      return '成功后被系统封禁'
    default:
      return `官方未定义的状态值 ${status}`
  }
}

/** 状态 → Element Plus 标签色：0 成功、1 进行中、5/6 是「成功后出事」单独一档警示。 */
export function publishStatusTagType(status: number | null | undefined): 'success' | 'warning' | 'info' | 'danger' {
  switch (status) {
    case 0:
      return 'success'
    case 1:
      return 'info'
    case 5:
    case 6:
      return 'warning'
    case 2:
    case 3:
    case 4:
      return 'danger'
    default:
      return 'info'
  }
}
