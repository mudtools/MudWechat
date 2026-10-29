<script setup lang="ts">
import { ref, watch } from 'vue'

import { listRevisions } from '../api/articles'
import type { RevisionView } from '../api/types'
import { formatIso } from '../utils/format'
import { presentError } from '../utils/notify'

const visible = defineModel<boolean>('visible', { required: true })

const props = defineProps<{ mediaId: string; index: number }>()
const emit = defineEmits<{ load: [revision: RevisionView] }>()

const items = ref<RevisionView[]>([])
const loading = ref(false)
const hint = ref('')

async function load() {
  if (!props.mediaId) {
    return
  }

  loading.value = true
  try {
    const result = await listRevisions(props.mediaId, props.index)
    items.value = result.items
    hint.value = result.hint ?? ''
  } catch (reason) {
    items.value = []
    presentError(reason)
  } finally {
    loading.value = false
  }
}

watch(visible, (opened) => {
  if (opened) {
    void load()
  }
})
</script>

<template>
  <el-drawer
    :model-value="visible"
    title="版本快照（本地台账，只读）"
    size="50%"
    @update:model-value="visible = $event as boolean"
  >
    <span class="ledger-badge">{{ hint || '本地记录，微信为准' }}</span>

    <div v-loading="loading" style="margin-top: 10px">
      <el-empty v-if="!items.length && !loading" description="本地还没有该篇的快照" :image-size="80" />

      <div v-for="revision in items" :key="revision.revisionId" style="margin-bottom: 12px">
        <el-card shadow="never">
          <template #header>
            <div style="display: flex; align-items: center; gap: 8px">
              <el-tag size="small" :type="revision.source === 'Create' ? 'success' : 'warning'">
                v{{ revision.revisionNo }} · {{ revision.source }}
              </el-tag>
              <el-text size="small">{{ revision.title }}</el-text>
              <span style="flex: 1" />
              <el-button size="small" text type="primary" @click="emit('load', revision)">
                载入编辑器
              </el-button>
            </div>
          </template>
          <el-descriptions :column="1" size="small" border>
            <el-descriptions-item label="摘要">{{ revision.digest || '—' }}</el-descriptions-item>
            <el-descriptions-item label="封面">
              <code>{{ revision.thumbMediaId ? '…' + revision.thumbMediaId.slice(-8) : '—' }}</code>
            </el-descriptions-item>
            <el-descriptions-item label="正文长度">{{ (revision.content ?? '').length }} 字符</el-descriptions-item>
            <el-descriptions-item label="时间">{{ formatIso(revision.createdAt) }}</el-descriptions-item>
          </el-descriptions>
        </el-card>
      </div>
    </div>

    <el-alert
      type="info"
      :closable="false"
      show-icon
      title="「载入编辑器」只填表单，不会自动写回微信"
      description="避免一次误点就把草稿覆盖成历史版本；确认后仍需手动「保存修改」。"
    />
  </el-drawer>
</template>
