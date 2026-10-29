import { h } from 'vue'
import { ElMessage } from 'element-plus'

import { ApiError } from '../api/client'

/**
 * 统一错误呈现（方案 §6.7）。
 * 返回 ApiError 表示「这是 400 参数错误」，调用方据此就地标红；其余形态已弹过提示，返回 null。
 */
export function presentError(reason: unknown): ApiError | null {
  if (!(reason instanceof ApiError)) {
    ElMessage.error(String((reason as Error)?.message ?? reason))
    return null
  }

  if (reason.kind === 'validation') {
    // 后端 400 只回 `{error: "…"} `、不带字段名，「就地标红」无从谈起 —— 先给出可见提示，
    // 再把错误返回给调用方（调用方若知道是哪个字段，可自行标红；不接也至少有反馈）。
    ElMessage.warning(reason.message)
    return reason
  }

  ElMessage({
    duration: 7000,
    type: 'error',
    message: () =>
      h('div', null, [
        h('div', null, reason.message),
        h('div', { style: 'opacity:.75;font-size:12px;margin-top:2px' }, [
          reason.errorCode !== null ? `errorCode ${reason.errorCode}` : `HTTP ${reason.status}`,
          reason.hint ? ` · ${reason.hint}` : '',
        ]),
      ]),
  })

  return null
}
