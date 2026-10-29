// 后端 → 前端契约（方案 §4.3）。字段名一律 camelCase：
// ASP.NET Core 的最小 API 默认用 camelCase 序列化匿名对象与 record 属性。

export interface StoreSummary {
  provider: string
  /** 只有文件名与大小 —— 库内明文存正文，绝对路径不回显（§11.2）。 */
  path: string
  jobs: number
  revisions: number
}

export interface HealthInfo {
  status: string
  demo: string
  utcNow: string
  writeGate: 'enabled' | 'disabled'
  config: {
    appKey: string
    appIdTail: string
    modules: string[]
    limits: { pageMax: number }
    materials: { type: string; uploadModeled: boolean }
    publishPoll: { timeoutSeconds: number; maxAttempts: number; backoffMs: number[] }
    publishEventsModeled: boolean
    store: StoreSummary
  }
}

export interface DraftSummary {
  mediaId: string
  title: string | null
  author: string | null
  digest: string | null
  thumbMediaId: string | null
  itemCount: number
  updateTime: number
}

export interface DraftArticleView {
  index: number
  title: string | null
  author: string | null
  digest: string | null
  content: string | null
  contentSourceUrl: string | null
  thumbMediaId: string | null
  needOpenComment: number | null
  onlyFansCanComment: number | null
  draftUrl: string | null
}

export interface DraftDetail {
  mediaId: string
  articles: DraftArticleView[]
}

export interface PublishedSummary {
  articleId: string
  title: string | null
  author: string | null
  digest: string | null
  thumbUrl: string | null
  itemCount: number
  isDeleted: boolean | null
  updateTime: number
}

export interface PublishedArticleView {
  title: string | null
  author: string | null
  digest: string | null
  content: string | null
  contentSourceUrl: string | null
  thumbMediaId: string | null
  thumbUrl: string | null
  url: string | null
  isDeleted: boolean | null
}

export interface PublishedDetail {
  articleId: string
  newsItems: PublishedArticleView[]
}

export interface MaterialSummary {
  mediaId: string
  name: string | null
  url: string | null
  updateTime: number
}

export interface ArticleUrlItem {
  idx: number | null
  articleUrl: string | null
}

export interface ArticleDetail {
  count: number | null
  items: ArticleUrlItem[]
}

/** 提交/查询发布的响应（§4.3）。 */
export interface PublishResult {
  publishId: string
  publishStatus: number | null
  articleId: string | null
  articleDetail: ArticleDetail | null
  failIdx: number[] | null
  attempts?: number
}

export interface PublishSnapshot {
  status: number | null
  errorCode: number | null
  polledAt: string
  attemptNo: number
}

export interface JobView {
  publishId: string
  mediaId: string
  latestStatus: number | null
  articleId: string | null
  attempts: number
  submittedAt: string
  completedAt: string | null
  snapshots: PublishSnapshot[]
}

export interface RevisionView {
  revisionId: number
  mediaId: string
  index: number
  revisionNo: number
  title: string
  digest: string | null
  source: string
  createdAt: string
  content: string | null
  author: string | null
  contentSourceUrl: string | null
  thumbMediaId: string | null
  needOpenComment: number | null
  onlyFansCanComment: number | null
}

/** 编辑器表单 → 后端 ArticleInput。 */
export interface ArticleInput {
  title: string | null
  author: string | null
  digest: string | null
  content: string | null
  contentSourceUrl: string | null
  thumbMediaId: string | null
  needOpenComment: number | null
  onlyFansCanComment: number | null
}

export interface PagedResult<T> {
  items: T[]
  totalCount: number
  itemCount: number
  hint?: string
}

export interface ListResult<T> {
  items: T[]
  hint?: string
}
