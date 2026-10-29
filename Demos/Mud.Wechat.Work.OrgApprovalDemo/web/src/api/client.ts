/**
 * 错误三态：业务错误（官方 errcode → 502 信封）/ 参数错误（400）/ 传输错误（请求没发出去）。
 *
 * 后端刻意把两类错误分开：`UseWechatApiErrorGate` 处理**异常**形态（令牌链与手工通道），
 * `ApiOutcome` 处理 **errcode** 形态（声明式端点在 errcode != 0 时仍是 HTTP 200）。
 * 两者信封同形，故前端只需一套解析。
 */
export type ApiErrorKind = 'business' | 'validation' | 'transport'

export class ApiError extends Error {
  readonly kind: ApiErrorKind
  readonly status: number
  readonly errorCode: number | null
  readonly hint: string | null

  constructor(
    kind: ApiErrorKind,
    status: number,
    message: string,
    errorCode: number | null = null,
    hint: string | null = null,
  ) {
    super(message)
    this.kind = kind
    this.status = status
    this.errorCode = errorCode
    this.hint = hint
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
  if (text.length === 0) {
    return null
  }

  try {
    return JSON.parse(text)
  } catch {
    return null
  }
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
    if (typeof body?.errorCode === 'number') {
      throw new ApiError('business', response.status, body.errorMessage ?? '官方返回错误', body.errorCode, body.hint ?? null)
    }

    if (typeof body?.error === 'string') {
      throw new ApiError('validation', response.status, body.error)
    }

    if (typeof body?.hint === 'string') {
      throw new ApiError('business', response.status, body.hint, null, body.hint)
    }

    throw new ApiError('transport', response.status, `HTTP ${response.status}`)
  }

  return body as T
}

export const get = <T>(path: string): Promise<T> => request<T>(path)

export const post = <T>(path: string, body?: unknown): Promise<T> =>
  request<T>(path, { method: 'POST', body: body === undefined ? undefined : JSON.stringify(body) })

export const put = <T>(path: string, body?: unknown): Promise<T> =>
  request<T>(path, { method: 'PUT', body: body === undefined ? undefined : JSON.stringify(body) })

export const del = <T>(path: string): Promise<T> => request<T>(path, { method: 'DELETE' })
