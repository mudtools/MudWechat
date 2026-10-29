import { ApiError, request } from './client'
import type {
  ArticleInput,
  DraftDetail,
  DraftSummary,
  HealthInfo,
  JobView,
  ListResult,
  MaterialSummary,
  PagedResult,
  PublishResult,
  PublishedDetail,
  PublishedSummary,
  RevisionView,
} from './types'

const enc = encodeURIComponent

// —— 装配探针（无凭据也可用）——————————————————————————————————————————

export function getHealth(): Promise<HealthInfo> {
  return request<HealthInfo>('/healthz')
}

// —— 读面（不受写闸限制）——————————————————————————————————————————

export function listDrafts(offset: number, count: number): Promise<PagedResult<DraftSummary>> {
  return request(`/api/articles/drafts?offset=${offset}&count=${count}`)
}

export function getDraft(mediaId: string): Promise<DraftDetail> {
  return request(`/api/articles/drafts/${enc(mediaId)}`)
}

export function listPublished(offset: number, count: number): Promise<PagedResult<PublishedSummary>> {
  return request(`/api/articles/published?offset=${offset}&count=${count}`)
}

export function getPublished(articleId: string): Promise<PublishedDetail> {
  return request(`/api/articles/published/${enc(articleId)}`)
}

export function listMaterials(offset: number, count: number): Promise<PagedResult<MaterialSummary>> {
  return request(`/api/articles/materials?offset=${offset}&count=${count}`)
}

export function getPublishStatus(publishId: string): Promise<PublishResult> {
  return request(`/api/articles/publish/${enc(publishId)}`)
}

export function listJobs(limit = 50): Promise<ListResult<JobView>> {
  return request(`/api/articles/jobs?limit=${limit}`)
}

export function listRevisions(mediaId: string, index: number): Promise<ListResult<RevisionView>> {
  return request(`/api/articles/drafts/${enc(mediaId)}/articles/${index}/revisions`)
}

// —— 写面（闸关时后端不映射 ⇒ 404/405；前端据 /healthz 提前禁用按钮）——————

export function createDraft(articles: ArticleInput[]): Promise<{ mediaId: string; articleCount: number }> {
  return request('/api/articles/drafts', { method: 'POST', body: JSON.stringify({ articles }) })
}

export function updateDraft(
  mediaId: string,
  index: number,
  article: ArticleInput,
): Promise<{ mediaId: string; index: number; revisionNo: number }> {
  return request(`/api/articles/drafts/${enc(mediaId)}/articles/${index}`, {
    method: 'PUT',
    body: JSON.stringify(article),
  })
}

/** 删除不可逆：confirm 必须等于目标 id（与前端弹窗构成双重保护）。 */
export function deleteDraft(mediaId: string): Promise<{ mediaId: string; deleted: boolean }> {
  return request(`/api/articles/drafts/${enc(mediaId)}?confirm=${enc(mediaId)}`, { method: 'DELETE' })
}

export function submitPublish(mediaId: string): Promise<PublishResult> {
  return request('/api/articles/publish', { method: 'POST', body: JSON.stringify({ mediaId }) })
}

export function deletePublished(articleId: string, index: number): Promise<{ articleId: string; deleted: boolean }> {
  return request(`/api/articles/published/${enc(articleId)}?index=${index}&confirm=${enc(articleId)}`, {
    method: 'DELETE',
  })
}

/** 轮询到终态；服务端超时/次数用尽时不是失败，而是「仍在发布中」。 */
export interface WaitResult extends PublishResult {
  kind: 'terminal' | 'pending'
  message: string | null
}

export async function waitPublish(publishId: string, timeoutSeconds = 45): Promise<WaitResult> {
  try {
    const result = await request<PublishResult>(
      `/api/articles/publish/${enc(publishId)}/wait?timeoutSeconds=${timeoutSeconds}`,
    )

    return { ...result, kind: 'terminal', message: null }
  } catch (reason) {
    if (reason instanceof ApiError && reason.errorCode === null && reason.status === 502) {
      const payload = reason.payload as {
        publishId?: string
        publishStatus?: number | null
        attempts?: number
        hint?: string
      } | null

      if (payload?.publishId) {
        return {
          kind: 'pending',
          publishId: payload.publishId,
          publishStatus: payload.publishStatus ?? null,
          articleId: null,
          articleDetail: null,
          failIdx: null,
          attempts: payload.attempts ?? 0,
          message: payload.hint ?? reason.message,
        }
      }
    }

    throw reason
  }
}
