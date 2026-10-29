<script setup lang="ts">
import { computed, ref, watch } from 'vue'

import { getPublishStatus, waitPublish } from '../api/articles'
import type { PublishResult } from '../api/types'
import { publishStatusTagType, publishStatusText } from '../utils/format'
import { presentError } from '../utils/notify'

const visible = defineModel<boolean>('visible', { required: true })

const props = defineProps<{ publishId: string }>()

const result = ref<PublishResult | null>(null)
const message = ref('')
const loading = ref(false)
const waiting = ref(false)

const status = computed(() => result.value?.publishStatus ?? null)
const isTerminal = computed(() => status.value !== null && status.value !== 1)
const step = computed(() => {
  if (status.value === null) {
    return 0
  }

  return status.value === 1 ? 1 : 2
})

async function refresh() {
  if (!props.publishId) {
    return
  }

  loading.value = true
  message.value = ''
  try {
    result.value = await getPublishStatus(props.publishId)
  } catch (reason) {
    const handled = presentError(reason)
    if (handled) {
      message.value = handled.message
    }
  } finally {
    loading.value = false
  }
}

async function waitUntilTerminal() {
  if (!props.publishId) {
    return
  }

  waiting.value = true
  message.value = ''
  try {
    const outcome = await waitPublish(props.publishId, 45)
    result.value = outcome
    message.value = outcome.kind === 'pending' ? outcome.message ?? '仍在发布中' : ''
  } catch (reason) {
    const handled = presentError(reason)
    if (handled) {
      message.value = handled.message
    }
  } finally {
    waiting.value = false
  }
}

watch(
  () => [visible.value, props.publishId],
  ([opened]) => {
    if (opened) {
      void refresh()
    }
  },
)
</script>

<template>
  <el-dialog
    :model-value="visible"
    title="发布状态"
    width="640px"
    @update:model-value="visible = $event as boolean"
  >
    <el-steps :active="step" align-center finish-status="wait" style="margin-bottom: 16px">
      <el-step title="提交" description="freepublish/submit 已受理" />
      <el-step title="发布中" description="轮询 freepublish/get" />
      <el-step :title="isTerminal ? '已出结论' : '未完成'" :description="publishStatusText(status)" />
    </el-steps>

    <div style="display: flex; gap: 8px; align-items: center; flex-wrap: wrap">
      <el-tag size="small">publish_id …{{ props.publishId.slice(-10) }}</el-tag>
      <el-tag size="small" :type="publishStatusTagType(status)">
        {{ status === null ? '尚未取到状态' : `${status} · ${publishStatusText(status)}` }}
      </el-tag>
      <el-text v-if="result?.articleId" size="small">
        article_id：<code>…{{ result.articleId.slice(-8) }}</code>
      </el-text>
    </div>

    <el-alert
      v-if="message"
      type="warning"
      :closable="false"
      show-icon
      :title="message"
      style="margin-top: 12px"
    />

    <div v-if="result?.articleDetail?.items?.length" style="margin-top: 12px">
      <div v-for="item in result.articleDetail.items" :key="item.idx ?? undefined">
        <el-link :href="item.articleUrl ?? ''" target="_blank" rel="noopener">
          第 {{ (item.idx ?? 0) + 1 }} 篇 · {{ item.articleUrl }}
        </el-link>
      </div>
    </div>

    <div v-if="result?.failIdx?.length" style="margin-top: 12px">
      <el-text size="small" type="danger">
        fail_idx：{{ result.failIdx.join(', ') }}（官方给出的失败篇章下标）
      </el-text>
    </div>

    <el-text size="small" type="info" style="display: block; margin-top: 12px">
      官方未向 API 开放发布结果事件（SDK 也未建模 PUBLISHJOBFINISH）⇒ 只能轮询；
      每轮状态都已落进「发布历史」Tab 的台账，进程重启后仍可续查。
    </el-text>

    <template #footer>
      <el-button :loading="loading" @click="refresh">查一次</el-button>
      <el-button type="primary" :loading="waiting" @click="waitUntilTerminal">
        等到终态（最多 45s）
      </el-button>
      <el-button @click="visible = false">关闭</el-button>
    </template>
  </el-dialog>
</template>
