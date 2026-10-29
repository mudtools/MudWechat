<script setup lang="ts">
import { computed, onMounted, reactive, ref } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Refresh } from '@element-plus/icons-vue'

import {
  createDraft,
  deleteDraft,
  getDraft,
  getHealth,
  submitPublish,
  updateDraft,
} from './api/articles'
import type { ArticleInput, HealthInfo } from './api/types'
import { presentError } from './utils/notify'
import ArticleEditor from './components/ArticleEditor.vue'
import DraftPanel from './components/DraftPanel.vue'
import PublishedPanel from './components/PublishedPanel.vue'
import JobsPanel from './components/JobsPanel.vue'
import PublishStatusDialog from './components/PublishStatusDialog.vue'

const emptyForm = (): ArticleInput => ({
  title: null,
  author: null,
  digest: null,
  content: null,
  contentSourceUrl: null,
  thumbMediaId: null,
  needOpenComment: 0,
  onlyFansCanComment: 0,
})

const health = ref<HealthInfo | null>(null)
const writeEnabled = computed(() => health.value?.writeGate === 'enabled')
const pageMax = computed(() => health.value?.config.limits.pageMax ?? 20)

const activeTab = ref<'drafts' | 'published' | 'jobs'>('drafts')
const draftCount = ref(0)
const publishedCount = ref(0)
const draftRefreshKey = ref(0)

const form = ref<ArticleInput>(emptyForm())

/** 编辑器当前绑定的草稿：mediaId 为空串 = 尚未落进微信草稿箱的新稿。 */
const editing = reactive({ mediaId: '', articleCount: 1, draftUrl: '' })

const status = reactive({ visible: false, publishId: '' })

async function loadHealth() {
  try {
    health.value = await getHealth()
  } catch (reason) {
    presentError(reason)
  }
}

function startNewDraft() {
  form.value = emptyForm()
  editing.mediaId = ''
  editing.articleCount = 1
  editing.draftUrl = ''
}

async function selectDraft(mediaId: string) {
  try {
    const detail = await getDraft(mediaId)
    const first = detail.articles[0]
    if (!first) {
      ElMessage.warning('该草稿没有任何篇章')
      return
    }

    form.value = {
      title: first.title,
      author: first.author,
      digest: first.digest,
      content: first.content,
      contentSourceUrl: first.contentSourceUrl,
      thumbMediaId: first.thumbMediaId,
      needOpenComment: first.needOpenComment,
      onlyFansCanComment: first.onlyFansCanComment,
    }
    editing.mediaId = detail.mediaId
    editing.articleCount = detail.articles.length
    editing.draftUrl = first.draftUrl ?? ''

    if (detail.articles.length > 1) {
      ElMessage.info(`该草稿含 ${detail.articles.length} 篇，本页只编辑第 1 篇`)
    }
  } catch (reason) {
    presentError(reason)
  }
}

async function saveNewDraft() {
  if (!form.value.thumbMediaId) {
    ElMessage.warning('请先从素材库选一张封面（news 图文必填 thumb_media_id）')
    return
  }

  try {
    const result = await createDraft([form.value])
    editing.mediaId = result.mediaId
    editing.articleCount = result.articleCount
    draftRefreshKey.value += 1
    ElMessage.success(`草稿已创建：…${result.mediaId.slice(-8)}`)
  } catch (reason) {
    presentError(reason)
  }
}

async function saveDraft() {
  if (!editing.mediaId) {
    ElMessage.warning('当前是新建稿：请先「保存为新草稿」')
    return
  }

  try {
    const result = await updateDraft(editing.mediaId, 0, form.value)
    draftRefreshKey.value += 1
    ElMessage.success(`已保存（本地第 ${result.revisionNo} 版快照）`)
  } catch (reason) {
    presentError(reason)
  }
}

async function removeDraft() {
  const mediaId = editing.mediaId
  if (!mediaId) {
    ElMessage.warning('当前是新建稿，微信侧还没有可删的对象')
    return
  }

  // 双重保护：弹窗确认 + 后端 confirm 参数（§11.4）。
  try {
    await ElMessageBox.confirm(
      `删除草稿不可逆（本地版本快照会保留，微信草稿箱里的它不会）。目标：…${mediaId.slice(-8)}`,
      '确认删除草稿',
      { type: 'warning', confirmButtonText: '我确定要删除', cancelButtonText: '取消' },
    )
  } catch {
    return
  }

  try {
    await deleteDraft(mediaId)
    draftRefreshKey.value += 1
    startNewDraft()
    ElMessage.success('草稿已删除')
  } catch (reason) {
    presentError(reason)
  }
}

async function publishDraft() {
  if (!editing.mediaId) {
    ElMessage.warning('请先保存为草稿（发布需要 media_id）')
    return
  }

  try {
    const result = await submitPublish(editing.mediaId)
    status.publishId = result.publishId
    status.visible = true
  } catch (reason) {
    presentError(reason)
  }
}

function openStatus() {
  if (!status.publishId) {
    ElMessage.info('本会话还没有发布任务；请到「发布历史」Tab 选一条查看')
    return
  }

  status.visible = true
}

onMounted(loadHealth)
</script>

<template>
  <el-container style="height: 100%">
    <el-header class="app-header" height="56px">
      <span class="app-header__title">公众号文章管理 Demo</span>
      <el-tag size="small" type="info">
        AppId …{{ health?.config.appIdTail ?? '未读取' }}
      </el-tag>
      <el-tag size="small" :type="writeEnabled ? 'warning' : 'success'">
        写闸：{{ writeEnabled ? '已放行（会写真账号）' : '关闭' }}
      </el-tag>
      <span class="app-header__spacer" />
      <el-button :icon="Refresh" size="small" @click="loadHealth">刷新</el-button>
    </el-header>

    <el-container class="app-body">
      <el-aside width="420px" class="app-aside">
        <el-alert
          v-if="!writeEnabled"
          type="info"
          :closable="false"
          show-icon
          title="写操作端点未映射"
          description="设置 MUDWECHAT_DEMO_ALLOW_WRITE=1 后重启，才会映射创建/编辑/发布/删除端点。"
          style="margin: 8px 0"
        />
        <el-alert
          v-else
          type="warning"
          :closable="false"
          show-icon
          title="当前会向真实公众号写入数据"
          description="确认指向测试号；删除草稿与撤下文章均不可逆。"
          style="margin: 8px 0"
        />

        <el-tabs v-model="activeTab">
          <el-tab-pane :label="`草稿 (${draftCount})`" name="drafts">
            <DraftPanel
              :write-enabled="writeEnabled"
              :page-max="pageMax"
              :refresh-key="draftRefreshKey"
              :active-media-id="editing.mediaId"
              @select="selectDraft"
              @new-draft="startNewDraft"
              @count="draftCount = $event"
            />
          </el-tab-pane>

          <el-tab-pane :label="`已发布 (${publishedCount})`" name="published">
            <PublishedPanel
              :write-enabled="writeEnabled"
              :page-max="pageMax"
              @count="publishedCount = $event"
            />
          </el-tab-pane>

          <el-tab-pane label="发布历史" name="jobs">
            <JobsPanel @inspect="status.publishId = $event; status.visible = true" />
          </el-tab-pane>
        </el-tabs>
      </el-aside>

      <el-main class="app-main">
        <ArticleEditor
          v-model="form"
          :write-enabled="writeEnabled"
          :media-id="editing.mediaId"
          :article-count="editing.articleCount"
          :draft-url="editing.draftUrl"
          @save-new="saveNewDraft"
          @save="saveDraft"
          @remove="removeDraft"
          @publish="publishDraft"
          @status="openStatus"
        />
      </el-main>
    </el-container>

    <PublishStatusDialog v-model:visible="status.visible" :publish-id="status.publishId" />
  </el-container>
</template>
