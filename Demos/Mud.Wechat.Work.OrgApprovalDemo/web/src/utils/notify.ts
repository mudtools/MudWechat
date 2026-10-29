import { ElMessage } from 'element-plus'
import { ApiError } from '../api/client'

/**
 * 统一的错误呈现：业务错误带官方错误码与提示；参数错误只给一句人话；传输错误提示后端没起。
 */
export function presentError(reason: unknown): void {
  if (reason instanceof ApiError) {
    if (reason.kind === 'business') {
      const code = reason.errorCode === null ? '' : `[${reason.errorCode}] `
      ElMessage.error(`${code}${reason.message}`)
      if (reason.hint) {
        ElMessage({ type: 'info', message: reason.hint, duration: 6000, showClose: true })
      }
      return
    }

    ElMessage.error(reason.message)
    return
  }

  ElMessage.error((reason as Error)?.message ?? '未知错误')
}

export function presentSuccess(message: string): void {
  ElMessage.success(message)
}
