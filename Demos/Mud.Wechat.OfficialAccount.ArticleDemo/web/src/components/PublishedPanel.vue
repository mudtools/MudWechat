<script setup lang="ts">
import { onMounted, reactive, ref, watch } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import { Delete, Search } from '@element-plus/icons-vue'

import { deletePublished, getPublished, listPublished } from '../api/articles'
import type { PublishedDetail, PublishedSummary } from '../api/types'
import { tail } from '../utils/format'
import { presentError } from '../utils/notify'

const props = defineProps<{ writeEnabled: boolean; pageMax: number }>()
const emit = defineEmits<{ count: [value: number] }>()

const pageSize = 10
const items = ref<PublishedSummary[]>([])
const total = ref(0)
const currentPage = ref(1)
const loading = ref(false)

const detail = reactive({ visible: false, loading: false, data: null as PublishedDetail | null, articleId: '' })

async function load() {
  loading.value = true
  try {
    const page = await listPublished((currentPage.value - 1) * pageSize, Math.min(pageSize, props.pageMax))
    items.value = page.items
    total.value = page.totalCount
    emit('count', page.totalCount)
  } catch (reason) {
    items.value = []
    total.value = 0
    emit('count', 0)
    presentError(reason)
  } finally {
    loading.value = false
  }
}

async function openDetail(articleId: string) {
  detail.visible = true
  detail.loading = true
  detail.articleId = articleId
  detail.data = null
  try {
    detail.data = await getPublished(articleId)
  } catch (reason) {
    presentError(reason)
    detail.visible = false
  } finally {
    detail.loading = false
  }
}

async function unpublish() {
  const articleId = detail.articleId
  if (!articleId) {
    return
  }

  try {
    await ElMessageBox.confirm(
      '撤下已发布文章不可逆，且官方无「编辑已发布」接口 —— 改内容只能撤下后从草稿重发。目标：' + tail(articleId, 8),
      '确认撤下',
      { type: 'warning', confirmButtonText: '我确定要撤下', cancelButtonText: '取消' },
    )
  } catch {
    return
  }

  try {
    await deletePublished(articleId, 0)
    ElMessage.success('已提交撤下（index=0 = 删除该发布下的全部篇章）')
    detail.visible = false
    await load()
  } catch (reason) {
    presentError(reason)
  }
}

onMounted(load)
watch(() => props.pageMax, load)

/** 正文只截 plain text 预览：v-html 会把账号内容当 HTML 执行，Demo 里没有这个必要。 */
function excerpt(content: string | null): string {
  if (!content) {
    return '（空）'
  }

  return content.replace(/<[^>]+>/g, ' ').replace(/\s+/g, ' ').slice(0, 300)
}
</script>

<template>
  <div>
    <div class="panel__toolbar">
      <el-button :icon="Search" size="small" :loading="loading" @click="load">刷新</el-button>
      <span style="flex: 1" />
      <el-tag size="small" type="info">共 {{ total }} 篇</el-tag>
    </div>

    <el-table
      v-loading="loading"
      :data="items"
      size="small"
      @row-click="(row: PublishedSummary) => openDetail(row.articleId)"
    >
      <el-table-column prop="title" label="标题" min-width="150" show-overflow-tooltip />
      <el-table-column label="状态" width="70">
        <template #default="{ row }">
          <el-tag size="small" :type="row.isDeleted ? 'danger' : 'success'">
            {{ row.isDeleted ? '已删' : '在线' }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column label="id" width="80">
        <template #default="{ row }">{{ tail(row.articleId, 6) }}</template>
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

    <el-drawer v-model="detail.visible" title="已发布文章（只读）" size="55%">
      <el-alert
        type="info"
        :closable="false"
        show-icon
        title="官方没有 freepublish/update：已发布内容不可编辑"
        description="要改内容只能撤下（不可逆）后重发草稿。这里不提供任何编辑入口。"
      />

      <div v-loading="detail.loading" style="margin-top: 12px">
        <div v-for="(item, i) in detail.data?.newsItems ?? []" :key="i" style="margin-bottom: 16px">
          <el-divider content-position="left">第 {{ i + 1 }} 篇</el-divider>
          <el-descriptions :column="1" border size="small">
            <el-descriptions-item label="标题">{{ item.title || '—' }}</el-descriptions-item>
            <el-descriptions-item label="作者">{{ item.author || '—' }}</el-descriptions-item>
            <el-descriptions-item label="摘要">{{ item.digest || '—' }}</el-descriptions-item>
            <el-descriptions-item label="正文（纯文本截断）">{{ excerpt(item.content) }}</el-descriptions-item>
            <el-descriptions-item label="线上地址">
              <el-link v-if="item.url" :href="item.url" target="_blank" rel="noopener">{{ item.url }}</el-link>
              <span v-else>—</span>
            </el-descriptions-item>
            <el-descriptions-item label="封面">
              <el-image v-if="item.thumbUrl" :src="item.thumbUrl" style="max-height: 120px" fit="contain" />
              <span v-else>—</span>
            </el-descriptions-item>
          </el-descriptions>
        </div>
      </div>

      <el-button type="danger" :icon="Delete" :disabled="!props.writeEnabled" @click="unpublish">
        撤下该文章（不可逆）
      </el-button>
      <el-text v-if="!props.writeEnabled" size="small" type="info">写闸关闭时该按钮无效</el-text>
    </el-drawer>
  </div>
</template>
