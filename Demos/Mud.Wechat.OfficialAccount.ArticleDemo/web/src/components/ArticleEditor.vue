<script setup lang="ts">
import { computed, ref } from 'vue'
import { Delete, Document, Files, Picture, Promotion, Tickets, Timer } from '@element-plus/icons-vue'
import type { InputInstance } from 'element-plus'

import type { ArticleInput, MaterialSummary, RevisionView } from '../api/types'
import MaterialPicker from './MaterialPicker.vue'
import MaterialDrawer from './MaterialDrawer.vue'
import RevisionDrawer from './RevisionDrawer.vue'

const article = defineModel<ArticleInput>({ required: true })

const props = defineProps<{
  writeEnabled: boolean
  mediaId: string
  articleCount: number
  draftUrl: string
}>()

const emit = defineEmits<{
  saveNew: []
  save: []
  remove: []
  publish: []
  status: []
}>()

const contentRef = ref<InputInstance | null>(null)
const materialState = ref<{ visible: boolean; mode: 'cover' | 'content' }>({ visible: false, mode: 'cover' })
const revisionVisible = ref(false)

/** 封面预览 URL 只在本次会话里由素材选择器带出：官方接口不给「media_id → url」的单独反查。 */
const coverUrl = ref('')

// 官方字段上限：标题 ≤32 字、摘要 ≤120 字（守卫 CT2 锁定的官方原文）；作者按 UI 约定限 16。
const title = computed({
  get: () => article.value.title ?? '',
  set: (value: string) => {
    article.value.title = value.length > 0 ? value : null
  },
})
const author = computed({
  get: () => article.value.author ?? '',
  set: (value: string) => {
    article.value.author = value.length > 0 ? value : null
  },
})
const digest = computed({
  get: () => article.value.digest ?? '',
  set: (value: string) => {
    article.value.digest = value.length > 0 ? value : null
  },
})
const contentSourceUrl = computed({
  get: () => article.value.contentSourceUrl ?? '',
  set: (value: string) => {
    article.value.contentSourceUrl = value.length > 0 ? value : null
  },
})
const content = computed({
  get: () => article.value.content ?? '',
  set: (value: string) => {
    article.value.content = value
  },
})
const needOpenComment = computed({
  get: () => article.value.needOpenComment === 1,
  set: (value: boolean) => {
    article.value.needOpenComment = value ? 1 : 0
  },
})
const onlyFansCanComment = computed({
  get: () => article.value.onlyFansCanComment === 1,
  set: (value: boolean) => {
    article.value.onlyFansCanComment = value ? 1 : 0
  },
})

const previewDoc = computed(
  () => `<!doctype html><html><head><meta charset="utf-8"><style>
body{font:14px/1.7 -apple-system,"Microsoft YaHei",sans-serif;margin:10px;word-break:break-word}
img{max-width:100%}</style></head><body>${content.value}</body></html>`,
)

function chooseCover(material: MaterialSummary) {
  article.value.thumbMediaId = material.mediaId
  coverUrl.value = material.url ?? ''
  materialState.value.visible = false
}

function insertContentImage(material: MaterialSummary) {
  const snippet = `<img src="${material.url ?? ''}" alt="${material.name ?? ''}" />`
  const current = content.value
  const textarea = contentRef.value?.textarea

  if (textarea) {
    const start = textarea.selectionStart ?? current.length
    const end = textarea.selectionEnd ?? current.length
    content.value = current.slice(0, start) + snippet + current.slice(end)

    requestAnimationFrame(() => {
      textarea.focus()
      const caret = start + snippet.length
      textarea.setSelectionRange(caret, caret)
    })
  } else {
    content.value = current + snippet
  }

  materialState.value.visible = false
}

function onMaterialChosen(material: MaterialSummary) {
  if (materialState.value.mode === 'cover') {
    chooseCover(material)
  } else {
    insertContentImage(material)
  }
}

function applyRevision(revision: RevisionView) {
  article.value.title = revision.title || null
  article.value.author = revision.author
  article.value.digest = revision.digest
  article.value.content = revision.content ?? ''
  article.value.contentSourceUrl = revision.contentSourceUrl
  article.value.thumbMediaId = revision.thumbMediaId
  article.value.needOpenComment = revision.needOpenComment
  article.value.onlyFansCanComment = revision.onlyFansCanComment
  coverUrl.value = ''
  revisionVisible.value = false
}
</script>

<template>
  <div>
    <el-alert
      v-if="props.articleCount > 1"
      type="info"
      :closable="false"
      show-icon
      :title="`该草稿含 ${props.articleCount} 篇，本页只编辑第 1 篇`"
      description="官方 draft/update 用 index 指定篇序号；多篇编辑超出本 Demo 范围（README 已登记为扩展点）。"
      style="margin-bottom: 12px"
    />

    <el-form label-width="88px" label-position="left">
      <el-form-item label="标题">
        <el-input v-model="title" maxlength="32" show-word-limit placeholder="必填，≤32 字" />
      </el-form-item>

      <el-form-item label="作者">
        <el-input v-model="author" maxlength="16" show-word-limit placeholder="选填" />
      </el-form-item>

      <el-form-item label="摘要">
        <el-input
          v-model="digest"
          type="textarea"
          :rows="2"
          maxlength="120"
          show-word-limit
          placeholder="选填，≤120 字；留空时官方抓正文前 54 字"
        />
      </el-form-item>

      <el-form-item label="原文链接">
        <el-input v-model="contentSourceUrl" placeholder="选填，须是本站或已授权域名（41039 = 不合法）" />
      </el-form-item>

      <el-form-item label="封面">
        <MaterialPicker
          :thumb-media-id="article.thumbMediaId ?? ''"
          :preview-url="coverUrl"
          @open="materialState = { visible: true, mode: 'cover' }"
        />
      </el-form-item>

      <el-form-item label="正文">
        <div style="width: 100%">
          <el-input
            ref="contentRef"
            v-model="content"
            type="textarea"
            :rows="12"
            placeholder="正文 HTML。图片只能用素材库里的 url —— 站外图片会被官方过滤"
          />
          <div style="margin-top: 6px; display: flex; gap: 8px; align-items: center">
            <el-button
              size="small"
              :icon="Picture"
              @click="materialState = { visible: true, mode: 'content' }"
            >
              从素材库插图
            </el-button>
            <el-text size="small" type="info">插入到光标处；官方剥离 JS，预览与线上渲染会有差异</el-text>
          </div>
        </div>
      </el-form-item>

      <el-form-item label="评论">
        <div style="display: flex; gap: 24px">
          <el-switch v-model="needOpenComment" active-text="打开留言" />
          <el-switch v-model="onlyFansCanComment" active-text="仅粉丝可留言" />
        </div>
      </el-form-item>

      <el-form-item label="预览">
        <iframe class="editor-preview" sandbox="" :srcdoc="previewDoc" title="正文预览" />
      </el-form-item>

      <el-form-item label="草稿链接">
        <el-link v-if="props.draftUrl" :href="props.draftUrl" target="_blank" rel="noopener">
          微信侧草稿预览
        </el-link>
        <el-text v-else size="small" type="info">保存后由官方返回</el-text>
      </el-form-item>
    </el-form>

    <el-divider />

    <div style="display: flex; gap: 8px; flex-wrap: wrap">
      <el-button type="primary" :icon="Document" :disabled="!props.writeEnabled" @click="emit('saveNew')">
        保存为新草稿
      </el-button>
      <el-button :icon="Files" :disabled="!props.writeEnabled || !props.mediaId" @click="emit('save')">
        保存修改
      </el-button>
      <el-button type="success" :icon="Promotion" :disabled="!props.writeEnabled || !props.mediaId" @click="emit('publish')">
        发布
      </el-button>
      <el-button :icon="Timer" :disabled="!props.mediaId" @click="emit('status')">查看状态</el-button>
      <el-button :icon="Tickets" :disabled="!props.mediaId" @click="revisionVisible = true">历史版本</el-button>
      <el-button type="danger" :icon="Delete" :disabled="!props.writeEnabled || !props.mediaId" @click="emit('remove')">
        删除草稿
      </el-button>
    </div>

    <el-text v-if="!props.writeEnabled" size="small" type="info" style="margin-top: 8px; display: block">
      写闸关闭：以上写操作端点未映射，按钮已禁用（不是点了才知道 404）。
    </el-text>

    <MaterialDrawer
      v-model:visible="materialState.visible"
      :mode="materialState.mode"
      @choose="onMaterialChosen"
    />

    <RevisionDrawer
      v-model:visible="revisionVisible"
      :media-id="props.mediaId"
      :index="0"
      @load="applyRevision"
    />
  </div>
</template>
