/** 错误三态（方案 §6.7）：业务错误（官方 errcode）/ 参数错误（400，就地标红）/ 传输错误（请求没发出去）。 */
export type ApiErrorKind = 'business' | 'validation' | 'transport'

export class ApiError extends Error {
  readonly kind: ApiErrorKind
  readonly status: number
  readonly errorCode: number | null
  readonly hint: string | null
  readonly payload: unknown

  constructor(
    kind: ApiErrorKind,
    status: number,
    message: string,
    errorCode: number | null = null,
    hint: string | null = null,
    payload: unknown = null,
  ) {
    super(message)
    this.kind = kind
    this.status = status
    this.errorCode = errorCode
    this.hint = hint
    this.payload = payload
  }
}

interface ErrorBody {
  errorCode?: number
  errorMessage?: string
  error?: string
  hint?: string
}

async function readBody(response: Response): Promise<unknown> {
  const text = await response.text()
  return text.length === 0 ? null : JSON.parse(text)
}

export async function request<T>(path: string, init?: RequestInit): Promise<T> {
  let response: Response
  try {
    response = await fetch(path, { headers: { 'Content-Type': 'application/json' }, ...init })
  } catch (reason) {
    throw new ApiError('transport', 0, `请求未能发出：${(reason as Error).message}（后端起着吗？）`)
  }

  const body = (await readBody(response)) as (ErrorBody & Record<string, unknown>) | null

  if (!response.ok) {
    // 502 + errorCode：官方拒了这次调用 ⇒ ElMessage.error 显示 errmsg，右侧标签显示错误码。
    if (typeof body?.errorCode === 'number') {
      throw new ApiError(
        'business',
        response.status,
        body.errorMessage ?? '官方返回错误',
        body.errorCode,
        body.hint ?? null,
        body,
      )
    }

    // 400：参数/confirm 不匹配 ⇒ 表单域就地标红，不弹全局错误。
    if (typeof body?.error === 'string') {
      throw new ApiError('validation', response.status, body.error, null, null, body)
    }

    // 无 errorCode 的 502 = 「轮询到超时仍在发布中」，语义不是失败，payload 里带着当前状态。
    if (typeof body?.hint === 'string') {
      throw new ApiError('business', response.status, body.hint, null, body.hint, body)
    }

    throw new ApiError('transport', response.status, `HTTP ${response.status}`)
  }

  return body as T
}
