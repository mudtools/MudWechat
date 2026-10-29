<script setup lang="ts">
import { onMounted, ref, watch } from 'vue'
import { Plus, Search } from '@element-plus/icons-vue'

import { listDrafts } from '../api/articles'
import type { DraftSummary } from '../api/types'
import { formatUnix, tail } from '../utils/format'
import { presentError } from '../utils/notify'

const props = defineProps<{
  writeEnabled: boolean
  pageMax: number
  refreshKey: number
  activeMediaId: string
}>()

const emit = defineEmits<{
  select: [mediaId: string]
  newDraft: []
  count: [value: number]
}>()

const pageSize = 10
const items = ref<DraftSummary[]>([])
const total = ref(0)
const currentPage = ref(1)
const loading = ref(false)

async function load() {
  loading.value = true
  try {
    const page = await listDrafts((currentPage.value - 1) * pageSize, Math.min(pageSize, props.pageMax))
    items.value = page.items
    total.value = page.totalCount
    emit('count', page.totalCount)
  } catch (reason) {
    // 假凭据下这里是 502 + 40013 —— 正是「请求确实走到了 api.weixin.qq.com」的证明。
    items.value = []
    total.value = 0
    emit('count', 0)
    presentError(reason)
  } finally {
    loading.value = false
  }
}

onMounted(load)
watch(() => props.refreshKey, load)
</script>

<template>
  <div>
    <div class="panel__toolbar">
      <el-button :icon="Search" size="small" :loading="loading" @click="load">刷新</el-button>
      <el-button :icon="Plus" size="small" type="primary" :disabled="!writeEnabled" @click="emit('newDraft')">
        新建草稿
      </el-button>
      <span style="flex: 1" />
      <el-tag size="small" type="info">共 {{ total }} 篇</el-tag>
    </div>

    <el-table
      v-loading="loading"
      :data="items"
      size="small"
      highlight-current-row
      @row-click="(row: DraftSummary) => emit('select', row.mediaId)"
    >
      <el-table-column prop="title" label="标题" min-width="150" show-overflow-tooltip>
        <template #default="{ row }">
          <span>{{ row.title || '（无标题）' }}</span>
        </template>
      </el-table-column>
      <el-table-column label="更新" width="90">
        <template #default="{ row }">{{ formatUnix(row.updateTime) }}</template>
      </el-table-column>
      <el-table-column label="id" width="90">
        <template #default="{ row }">
          <el-text size="small" :type="row.mediaId === props.activeMediaId ? 'primary' : 'secondary'">
            {{ tail(row.mediaId, 6) }}
          </el-text>
        </template>
      </el-table-column>
    </el-table>

    <div class="panel__pager">
      <el-pagination
        v-model:current-page="currentPage"
        :page-size="pageSize"
        :total="total"
        layout="prev, pager, next"
        size="small"
        @current-change="load"
      />
    </div>

    <el-text size="small" type="info">
      官方列表接口不分「标题」字段，标题取自 content.news_item[0].title ⇒ 请求必须 no_content=0。
    </el-text>
  </div>
</template>
